using System;
using AnnulusGames.SceneSystem;
using Features.Title.Presentation.Interfaces;
using Infrastructure.Services;
using R3;
using SharedPresentation.Interfaces;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class TitleScenePresenter : IPostInitializable, IDisposable
    {
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly ProgressDataManager progressDataManager;
        [Inject] private readonly ITitleView titleView;

        private readonly CompositeDisposable _disposables = new();

        public void PostInitialize()
        {
            if (titleView is IGenericView view) view.Show();

            Observable.FromEvent(
                    h => titleView.ToSettingsButton.onPointerUp += h,
                    h => titleView.ToSettingsButton.onPointerUp -= h)
                .Subscribe(_ =>
                {
                    if (sceneService.peekSceneKey != SceneKey.Title) return;
                    sceneService.PushScene(SceneKey.Settings, true);
                })
                .AddTo(_disposables);

            Observable.FromEvent(
                    h => titleView.ToCreditButton.onPointerUp += h,
                    h => titleView.ToCreditButton.onPointerUp -= h)
                .Subscribe(_ =>
                {
                    if (sceneService.peekSceneKey != SceneKey.Title) return;
                    sceneService.PushScene(SceneKey.Credit, true);
                })
                .AddTo(_disposables);

            Observable.FromEvent(
                    h => titleView.ToMainMenuButton.onPointerUp += h,
                    h => titleView.ToMainMenuButton.onPointerUp -= h)
                .Subscribe(_ =>
                {
                    if (sceneService.peekSceneKey != SceneKey.Title) return;
                    sceneService.PushScene(SceneKey.MainMenu, false);
                })
                .AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}
