# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

MirrorWorld is a Unity 2D roguelite deck-builder game — a turn-based strategy game on a 1D (single-line) battlefield inspired by Shogun Showdown and Slay the Spire. The design document is in `outline.md`.

## Common Commands

This is a Unity project (URP, Unity 6). There is no CLI build system — all builds and Play Mode testing are done from the Unity Editor. Open the project in Unity 6 by pointing the Unity Hub at this folder.

There are no automated test runner commands. The `com.unity.test-framework` package is present; tests can be run via **Window > General > Test Runner** in the Editor.

## Architecture

### DI Framework: VContainer

Every scene has a `LifetimeScope` that configures its DI container. The hierarchy is:

- **`RootLifetimeScope`** (`Assets/Scripts/Infrastructure/DI/RootLifetimeScope.cs`) — persistent singleton services registered once for the app lifetime: `SceneService`, `AudioService`, `ProgressDataManager`, `SettingsDataManager`, `AddressableAssetLoader`, `UIFocusManager`.
- **Per-scene `LifetimeScope`s** (e.g. `TitleSceneLifetimeScope`, `IngameSceneLifetimeScope`) — register the scene's Presenter and View bindings. All inherit from `BaseSceneLifetimeScope` which registers `BaseScenePresenter`.

### MVP Pattern

Each feature scene follows Model-View-Presenter:
- **View** — `MonoBehaviour` implementing an interface (`ITitleView`, `ISettingsView`, etc.). Lives in `Features/<Name>/Presentation/Views/`.
- **Presenter** — plain C# class implementing VContainer lifecycle interfaces (`IPostInitializable`, `IDisposable`). Lives in `Features/<Name>/Presentation/Presenters/`. Subscribes to View events using **R3** and calls Services.
- **Interface** — in `Features/<Name>/Presentation/Interfaces/`.

Shared UI components (buttons, checkboxes, progress bars) live in `Assets/Scripts/SharedPresentation/Views/`. `GenericButton` exposes C# `event Action` hooks (`onPointerUp`, `onClick`, etc.) that Presenters subscribe to via `Observable.FromEvent`.

### Reactive Programming: R3

All async event handling uses **R3** (not UniRx). Subscriptions are collected in `CompositeDisposable _disposables` and disposed in `IDisposable.Dispose()`. The `Infrastructure/Utilities/R3/` folder contains utility extensions: `PausableTimer` and `ReactiveStack<T>`.

### Scene Management

`SceneService` (`Infrastructure/Services/SceneService.cs`) manages a stack-based scene system built on `AnnulusGames.SceneSystem`:

- Scenes are identified by the `SceneKey` enum: `Base, Title, Settings, Credit, MainMenu, Ingame, StoryTelling, Gallery`.
- Scene names in the build must be `<SceneKey>Scene` (e.g. `TitleScene`).
- `BaseScene` is always loaded as the persistent root. Other scenes are additively pushed/popped.
- `PushScene(key, isDirect, needLoadingScreen)` — deferred to next `Tick()` to avoid mid-frame loads.
- Direct pushes (`isDirect=true`) use `Scenes.LoadSceneAsync` additively and are tracked in `directSceneList` for independent pop.
- `BaseScenePresenter` auto-pushes `peekSceneKey` on start — this is how the first scene after Base loads.

### Data Persistence

- `IProgressRepository` / `PlayerPrefsProgressRepository` — saves `GameProgressData` as JSON to `PlayerPrefs` under key `GameSaveData_V1`.
- `ProgressDataManager` — domain-level manager wrapping the repository. `GameMainProgress` is a `[Flags]` enum for bitfield progress tracking.
- `SettingsDataManager` — holds BGM/SE volume, language. Language switching uses **I2 Localization** (`LocalizationManager`).

### Platform & Resolution

`PlatformInitializer` runs at scene start to set orthographic camera size and `CanvasScaler` reference resolution based on `WorldSettings` (ScriptableObject) and the active `PlatformSettings`. `WorldSettings` is `1920×1080` at 100 PPU. `PlatformSettings` is selected at runtime by matching `Application.platform`; in Editor it falls back to `EditorUserBuildSettings.activeBuildTarget`.

### Audio

`AudioService` wraps **KanKikuchi AudioManager** (`Assets/AudioManager/`). BGM paths are prefixed with `BGM/`; SE paths with `SE/`. Audio files live in `Assets/AudioManager/Resources/BGM/` and `Assets/AudioManager/Resources/SE/`.

### Asset Loading

`AddressableAssetLoader` implements `IAssetLoader` and wraps Unity **Addressables** for runtime asset loading.

## Key Third-Party Packages

| Package | Purpose |
|---|---|
| VContainer 1.17.0 | Dependency injection |
| R3 (Cysharp) | Reactive programming (replaces UniRx) |
| UniTask (Cysharp) | Async/await |
| AnnulusGames.SceneSystem | Stack-based scene management |
| I2 Localization | Multi-language text |
| KanKikuchi AudioManager | BGM/SE management |
| Unity Addressables 2.9.1 | Asset bundle loading |
| Unity Input System 1.19.0 | Input handling |
| TextMesh Pro | UI text |

## Folder Conventions

```
Assets/Scripts/
  Infrastructure/
    DI/          # LifetimeScope registrations
    Services/    # App-wide services (SceneService, AudioService, etc.)
    Utilities/   # R3 helpers, InputActionExtensions, StringFormatter
  Features/
    <FeatureName>/
      Presentation/
        Interfaces/   # IView interfaces
        Views/        # MonoBehaviour views
        Presenters/   # Plain C# presenters
  SharedDomain/
    Models/        # ProgressDataManager, SettingsDataManager, etc.
    Repositories/  # IProgressRepository and implementations
  SharedPresentation/
    Interfaces/    # IGenericView
    Views/         # GenericButton, GenericCheckBox, GenericProgressBar, etc.
```
