using System;
using AnnulusGames.SceneSystem;
using Features.MainMenu.Presentation.Interfaces;
using Infrastructure.Services;
using R3;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class MainMenuScenePresenter : IPostInitializable, IDisposable
    {
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly IMainMenuView mainMenuView;

        private readonly CompositeDisposable _disposables = new();

        public void PostInitialize()
        {
            Observable.FromEvent(
                    h => mainMenuView.BackButton.onPointerUp += h,
                    h => mainMenuView.BackButton.onPointerUp -= h)
                .Subscribe(_ =>
                {
                    if (sceneService.peekSceneKey != SceneKey.MainMenu) return;
                    sceneService.PopScene();
                })
                .AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}
