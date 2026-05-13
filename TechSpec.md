# 技術仕様書テンプレート (Unity MVP + Clean Architecture)

> 本書はプロジェクト横断で再利用できる汎用アーキテクチャガイドです。
> プロジェクト固有の設計判断・実装詳細は別途 `ProjectSpec.md` に記録してください。

## 1. 技術アーキテクチャ

### 1.1 アプリケーションアーキテクチャ: MVP + Clean Architecture (Like)

機能ごとに凝集されたディレクトリ構成を採用し、Unityのライフサイクルとビジネスロジックを分離するために **MVP (Model-View-Presenter)** パターンを基本としています。

- **Layering**
    - **Presentation (View / Presenter)**: ユーザーインターフェースと入力ハンドリング。ビューは `MonoBehaviour` を継承しますが、ロジックは持ちません。
    - **Domain (Model)**: ゲームのルール、状態計算などの純粋なC#ロジック。UnityのAPIへの依存を極力排除します。
    - **Infrastructure**: データストア、外部API、ハードウェア固有機能へのアクセス。

### 1.2 推奨技術スタック

- **依存性注入 (DIContainer)**: **VContainer**
    - オブジェクトの生成と寿命管理、依存関係の解決を一元管理します。
    - `LifetimeScope` を使用して、シーン遷移や親コンテナからの継承スコープを制御します。

- **リアクティブプログラミング**: **R3 (Reactive Extensions for C#)**
    - 値の変化やイベントのストリームを監視し、Modelの状態変化をViewへ、Viewの入力をPresenterへ伝播させるために使用します。

- **コレクション・データ構造**: **ObservableCollections**
    - 高性能でリアクティブなコレクション（`ObservableList`, `ObservableDictionary` 等）を提供します。
    - リストの要素の追加・削除・入れ替えなどの変更を R3 の Observable として購読するために使用します。

- **非同期処理**: **UniTask**
    - ゼロアロケーションで高性能な非同期処理を実現します。
    - キャンセル処理（`CancellationToken`）の徹底により、オブジェクト破棄時のエラーを防止します。

- **Input System**: **Unity Input System**
    - `InputActions.inputactions` でアクション（Confirm, Cancel, Menu, Move, Select 等）を定義。
    - キーボード、ゲームパッド、マウス/タッチ入力に対応。
    - **Action Map切り替え**: ゲーム状態に応じて `Player` と `UI` のアクションマップを動的に切り替え。

- **UI Focus Management System**: **UIFocusGroup & UIFocusManager**
    - 複数のUIグループ間でのフォーカス管理をスタック構造で実現。
    - **UIFocusGroup** (View層): 各UIの塊に付与するコンポーネント。フォーカスの有効/無効を制御し、デフォルトフォーカス対象を管理。
    - **UIFocusManager** (Model層): R3のReactivePropertyを用いてUIフォーカススタックを管理。現在アクティブなUIグループをリアクティブに購読可能。
    - **設計方針**: MVPパターンに沿い、単方向データフローを維持。Presenterが `UIFocusManager.Push()/Pop()` でUI状態を変更し、ViewはそれをSubscribeして反映。
    - **用途**: ダイアログの多重表示、メニュー階層の管理、キーボード/ゲームパッドでのUI操作を想定。
    - **使用上の注意**:
        - UIFocusGroupは動的に生成されるUI要素を含む場合、要素生成後に `RefreshSelectables()` と `Activate()` を呼ぶ必要がある。
        - Unity UIのNavigationシステムは全てのinteractableなSelectableを対象とするため、フォーカス管理したいUI群は全てUIFocusGroupで管理することを推奨。
    - **DI登録**: シーン構成に応じて適切な `LifetimeScope` に登録する。additive マルチシーン構成では全シーンから参照可能にするため、上位スコープへの登録を推奨（→ ProjectSpec 参照）。

- **Resource Management**: **Addressable Asset System** (Hybrid)
    - **Asset-Link Only**: 重いアセット（アニメーション、BGM等）をAddressableで動的に読み込む。
    - `IAssetLoader` インターフェースを実装し、コンテナ破棄時に `Dispose` が呼ばれ読み込んだメモリが自動解放される仕組みを推奨。

### 1.3 ディレクトリ構成

機能単位（Feature）でフォルダを分割する構成を採用しています。

```text
Assets/
├── Scripts/
│   ├── Features/               # 機能別の実装
│   │   ├── [FeatureName]/      # 例: Title, Ingame など
│   │   │   ├── Domain/         # ロジック・データモデル
│   │   │   └── Presentation/   # UI・表示制御
│   │   │       ├── Interfaces/ # Viewへの抽象インターフェース
│   │   │       ├── Presenters/ # ViewとDomainを繋ぐクラス
│   │   │       └── Views/      # UIコンポーネント (MonoBehaviour)
│   │   └── ...
├── Infrastructure/             # データストア、外部サービス、シーン遷移管理
├── SharedDomain/               # 複数機能にまたがるModel・Repository
└── SharedPresentation/         # 複数機能にまたがるView・Presenter
```

---

## 2. 状態管理（State Management）のポリシー

データの不整合を防ぎ、デバッグ性を高めるため、以下の状態管理ルールを適用します。

### 2.1 単方向データフロー

- 状態の変更は **Domain (Model)** でのみ行い、**Presentation (View)** はその変更を購読して反映するだけに徹します。
- ViewからModelの値を直接書き換えることは禁止します。必ずPresenter/UseCaseを経由してModelのメソッドを介して変更します。

### 2.2 Read-Only プロパティの徹底

- Model内で保持する `ReactiveProperty<T>` は `private` または `protected` とし、外部（Presenter等）には `ReadOnlyReactiveProperty<T>` として公開します。
- これにより、どこから値が変更されたかをModel内に限定し、予期せぬ副作用を防止します。

---

## 3. コーディング規約・ベストプラクティス

### 3.1 UniTask とキャンセル処理

- 全ての非同期メソッド (`async UniTask`) は、原則として **`CancellationToken` を引数に受け取る** ように設計します。
- `MonoBehaviour` では `this.GetCancellationTokenOnDestroy()` を活用します。
- `VContainer` の `IStartable` 等で使用する場合は、`LifetimeScope` からトークンを注入するか、生成時に管理します。

### 3.2 R3 の購読管理

- `Subscribe` した際は必ず `.AddTo(compositeDisposable)` または `.AddTo(view)` を行い、寿命を管理します。
- ViewとPresenterが1対1の場合は、Viewの `OnDestroy` 時に一括でDisposeされることを保証します。

### 3.3 最新のC#機能の活用

- **Primary Constructors**: DIの記述を簡略化するために積極的に使用します。
- **File-scoped namespaces**: ネストを減らし可読性を向上させます。

---

## 4. 実装ガイドライン

### 4.1 クラス設計の責務

- **View**:
    - `MonoBehaviour` を継承。UI要素の参照保持と表示更新。
    - UniTaskを使用したアニメーション（待機を伴う表示処理）の実行。
- **Presenter**:
    - Plain C# Class (POCO) または `VContainer` の `IPostInitializable` などで起動。
    - `Inject` 属性で Model と View (Interface経由推奨) を受け取る。
    - Modelの状態を購読し、Viewを更新する。
    - Viewの入力を購読し、Modelのメソッドを叩く。
    - 複数の非同期処理を待機してViewを更新する場合、ここでUniTaskの制御を行います。
- **Model**:
    - ゲームの状態変数 (`ReactiveProperty` など) を持つ。
    - 計算ロジックを持つ。
    - ビジネスロジック。UniTaskを用いたデータ取得や永続化処理の結果をReactivePropertyに反映します。

### 4.2 Presenterの作成基準

#### Presenterに書くべき処理

Presenterの責務は「ViewのイベントをModelに伝える」「Modelの状態変化をViewに反映する」この2方向の配線に限定します。

| 書くべき処理 | 書くべきでない処理（他の層へ） |
|---|---|
| View → Model へのイベント伝達（ボタン購読など） | タイマー計算・ループ数判定などのロジック → **Model** |
| Model → View への状態反映（Subscribe） | ボタンのアニメーション・位置計算 → **View** |
| ViewのUniTask呼び出しと順序制御 | データの保存・読み込み → **Infrastructure** |
| 複数の非同期処理のシーケンス管理 | 複数機能にまたがるビジネスルール → **UseCase / Model** |

#### Presenterを新規作成するタイミング

**① 新しいViewができたとき**
ViewとPresenterは原則1対1で対応します。新しいViewが追加されたら、対応するPresenterを作ります。

**② 既存のPresenterが肥大化したとき**
「このPresenterが何をするクラスか一言で言えなくなったら」分割のサインです。UIパーツ単位でSub-Presenterに切り出します（→ 4.3参照）。

#### Presenterへの処理の帰属判断

ボタンが特定のViewに配置されていても、**反応ロジックが依存するドメイン**でどのPresenterに書くかを判断します。

```
ボタンの反応ロジックがFeature固有の型（Model / View）を使っているか？
    ↓ YES                         ↓ NO
そのFeatureのPresenterに書く    ボタンが属するViewのPresenterに書く
```

**実用的なチェック方法**: 処理を移動した結果、移動先のPresenterに無関係なドメインの型を `[Inject]` しなければならなくなったら、その移動は設計上の誤りです。

### 4.3 アーキテクチャ詳細・ベストプラクティス (Fat Presenter対策)

Presenterの責務過多（Fat Presenter）を防ぐため、以下のパターンを推奨します。

1. **Use Case (Interactor) パターンの導入**
    - Presenterに記述しがちな「一連のビジネスロジック（例: アイテム購入時の残高チェック〜インベントリ追加〜セーブ）」を、**Domain層の `UseCase` クラス** として切り出します。
    - Presenterは `UseCase.Execute()` を呼ぶだけの「交通整理役」に徹します。

2. **View/Presenterの分割 (Decomposition)**
    - 1つの画面を巨大なPresenterで管理せず、UIパーツ単位（ヘッダー、インベントリリスト、キャラステータス等）に **Sub-Presenter** を作成します。
    - 親Presenterは `LifetimeScope` を通じて子コンポーネントを初期化・接続します。

3. **Reactive Bindingの活用**
    - R3を活用し、Modelの変更を `Subscribe` して手動でViewにセットする手続き的コードを減らします。可能な限り宣言的にバインドします。

---

## 5. マルチプラットフォーム対応

プラットフォーム（Windows, WebGL, Android等）間の差異を吸収するため、以下の仕組みを推奨します。

### 5.1 PlatformSettings による抽象化

実行環境（`Application.platform`）に応じて設定を切り替える機能を ScriptableObject で実装します。

- **DisplaySettings**: 解像度、画面の向き、アスペクト比、CanvasScalerの設定。
- **InputSettings**: 入力デバイスの有効化、初期Input Action Mapの指定。
- **SystemSettings**: ターゲットフレームレート、VSync、画質レベル。

### 5.2 推奨初期化フロー

1. `RootLifetimeScope` が起動時に `Application.platform` に基づき適切な `PlatformSettings` アセットを選択。
2. DIコンテナに `PlatformSettings` のインスタンスとして登録。
3. `PlatformInitializer` が `EntryPoint` として起動し、画面・システム設定を適用。
4. 各シーンの `LifetimeScope` は親コンテナから設定を解決し、プラットフォームに応じたViewを表示。
