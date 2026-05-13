# MirrorWorld プロジェクト固有仕様

> 本書は [TechSpec.md](TechSpec.md) の汎用アーキテクチャガイドを前提とし、
> MirrorWorld プロジェクト固有の設計判断・実装詳細を記録します。

---

## 1. プロジェクト概要

ローグライトテイストのターン制デッキビルダーゲーム。Pixel Artスタイル。
Windows をメインターゲットとし、WebGL / Android / iOS を順次対応予定。

---

## 2. 開発環境

- Unity 6000.2.6f2（2D URP）
- C# 9.0（Unity制限）
- Git
- IDE: Antigravity および Rider

---

## 3. プロジェクト固有ライブラリ

TechSpec.md §1.2 の推奨スタックに加え、以下を導入している。

| ライブラリ | 用途 |
|---|---|
| AnnulusGames.SceneSystem | Stack式マルチシーン管理の基盤 |

> UniTask・Addressables・ObservableCollections については TechSpec.md §1.2 参照。

---

## 4. リリースプラットフォームと画面仕様

### 4.1 リリース優先度

1. Windows
2. WebGL
3. Android
4. iOS

### 4.2 共通仕様

- 基準ゲーム画面サイズ: **480×270**
- Pixel Per Unit: **1**（World・UIを含む全画像要素を 1px = 1単位で統一）
- 表示設定の種類：ウィンドウ×1 / ウィンドウ×2 / 全画面 の3段階

### 4.3 Windows

- 画面向き：横方向
- ×1表示: 480×270 ウィンドウ
- ×2表示: 960×540 ウィンドウ（全要素が2倍で表示）
- 全画面: 16:9ディスプレイは拡大表示。非16:9は縦サイズ基準で縦横等倍拡大
- 入力: マウス（将来: コントローラー対応）

### 4.4 WebGL

- 画面向き：横方向
- ×2表示のみ固定
- 入力: マウス（将来: コントローラー対応）

### 4.5 Android / iOS

- 画面向き：縦方向
- 全画面固定。次の特殊レイアウトを採用：
    - ゲーム画面をディスプレイ上部に配置し、専用UIを画面下部に配置
    - ゲーム画面はディスプレイ横幅に合わせて縦横等倍拡大
    - UIはその拡大倍率を適用後、ゲーム画面下端に上端を合わせてディスプレイ下端まで配置
- 入力: タッチ操作

---

## 5. マルチシーン構造

TechSpec.md §5 のプラットフォーム抽象化と合わせ、本プロジェクトでは **AnnulusGames.SceneSystem** をベースとした Stack 式マルチシーン階層構造を採用している。
詳細実装は `Assets/Scripts/Infrastructure/Services/SceneService.cs` を参照。

- **開発時**: Play実行時に自動で BaseScene が起動し、編集中のシーンがその上に Stack される
- **ビルド時**: BaseScene が起動シーンとして設定され、TitleScene → 以降のシーンへ順次遷移する

---

## 6. 設計判断ログ

### 6.1 Presenter-View 間イベント通信: Observable.FromEvent + CompositeDisposable

**判断**: View → Presenter の通知に `Observable.FromEvent` で C# `event Action` を R3 Observable へ変換し、`.AddTo(_disposables)` で購読ライフサイクルを管理する。

**理由**: `GenericButton` など View 層が `event Action` を公開する設計のため、`Observable.FromEvent` でラップすることで R3 の AddTo パターンをそのまま適用できる。Presenter が `IDisposable` を実装することで VContainer がスコープ破棄時に `Dispose()` を自動呼び出しし、全購読が一括解除される。

**実装パターン** (全 Presenter 共通):
```csharp
public class XxxPresenter : IPostInitializable, IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public void PostInitialize()
    {
        Observable.FromEvent(
                h => view.SomeButton.onPointerUp += h,
                h => view.SomeButton.onPointerUp -= h)
            .Subscribe(_ => /* 処理 */)
            .AddTo(_disposables);
    }

    public void Dispose() => _disposables.Dispose();
}
```

**適用済みファイル**: Title / MainMenu / Settings / Credit / Ingame / StoryTelling の各 ScenePresenter

---

### 6.2 UIFocusManager の DI 登録

**判断**: `RootLifetimeScope` で Singleton 登録。✅ 実装済み。

**理由**: Settings・Credit シーンが Ingame・Title シーンの上に additive ロードされる構成のため、シーン固有スコープに登録すると兄弟スコープからアクセスできない（TechSpec §1.2 の「additive マルチシーン構成では上位スコープへの登録を推奨」に準拠）。

```csharp
// RootLifetimeScope.cs
builder.Register<UIFocusManager>(Lifetime.Singleton);
```

---

### 6.3 IAssetLoader の DI 登録

**判断**: 現在は `RootLifetimeScope` に Singleton 登録。✅ 実装済み（暫定）。

```csharp
// RootLifetimeScope.cs
builder.Register<AddressableAssetLoader>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
```

**暫定理由**: アセット読み込みを使用するシーンが未確定のため、まず Root に Singleton で登録している。

**将来の方針**: 特定シーンでのみ使用することが確定した場合は、そのシーンの LifetimeScope に Scoped で移管する。Scoped にするとスコープ破棄時に `Dispose()` が呼ばれ、ロード済みアセットが自動解放される（TechSpec §1.2「コンテナ破棄時に Dispose が呼ばれ読み込んだメモリが自動解放」に準拠）。

---

### 6.4 async メソッドの CancellationToken 規約

**判断**: async メソッドには `CancellationToken cancellationToken = default` を引数として付与する。✅ 適用済み。

**適用ルール**:
- **MonoBehaviour 内の UniTaskVoid**: `this.GetCancellationTokenOnDestroy()` を取得して渡す。キャンセル時は `OperationCanceledException` をキャッチして早期 return。
- **Pure C# クラスのメソッド (IAssetLoader 等)**: `= default` でオプショナルにし、呼び出し側が任意でトークンを渡せるようにする。

```csharp
// MonoBehaviour (UIFocusGroup)
public void Activate()
{
    var token = this.GetCancellationTokenOnDestroy();
    SetFocusAsync(target, token).Forget();
}

private async UniTaskVoid SetFocusAsync(Selectable target, CancellationToken cancellationToken)
{
    try { await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken); }
    catch (OperationCanceledException) { return; }
    target.Select();
}

// Pure C# (IAssetLoader)
UniTask<T> LoadAssetAsync<T>(string key, CancellationToken cancellationToken = default);
```

**適用済みファイル**: `UIFocusGroup.cs` / `IAssetLoader.cs` / `AddressableAssetLoader.cs`

---

### 6.5 PlatformInitializer の Lifetime: Transient

**判断**: `RootLifetimeScope` で `Lifetime.Transient` として EntryPoint 登録。

```csharp
builder.RegisterEntryPoint<PlatformInitializer>(Lifetime.Transient);
```

**理由**: シーン遷移のたびに画面・システム設定を再適用するために毎回実行させる設計。Singleton にすると初回起動時のみ実行されるため不可。

---

### 6.6 PlatformSettings の構成フィールド

`SharedDomain/Models/PlatformSettings.cs` (ScriptableObject) に以下を統合定義:

- **目標プラットフォーム** (`RuntimePlatform targetPlatform`)
- **画面設定**: アスペクト比・向き・解像度
- **Canvas Scaler 設定**: `uiScaleMode`, `referenceResolution` 等

TechSpec §5.1 は DisplaySettings / InputSettings / SystemSettings への分離を推奨しているが、現プロジェクトでは単一クラスに統合している。将来的な分離は §6.5 の改善タスクとして残す。

---

### 6.7 R3 ユーティリティの提供

プロジェクト共通の R3 拡張を `Infrastructure/Utilities/R3/` に配置している。

| クラス | 概要 |
|---|---|
| `PausableTimer` | 一時停止・速度倍率変更が可能な非同期タイマー |
| `ReactiveStack<T>` | Push/Pop を Observable として購読できるスタック |

`InputActionExtensions` (`Infrastructure/Utilities/InputActionExtensions.cs`) は Unity Input System の `InputAction` を R3 `Observable` に変換するブリッジ（TechSpec §1.2 参照）。

---

### 6.8 BaseScenePresenter の ITickable

**判断**: `BaseScenePresenter` は `IStartable` と `ITickable` を実装しているが、`Tick()` は現在空実装。

**理由**: 将来的なフレーム毎処理の拡張余地を保持するために宣言している。不要と判断した時点で `ITickable` を削除する。
