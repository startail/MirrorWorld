using System;
using Infrastructure.Services;
using R3;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class StoryTellingScenePresenter : IPostInitializable, IDisposable
    {
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly GenericButton backButton;

        private readonly CompositeDisposable _disposables = new();

        public void PostInitialize()
        {
            Observable.FromEvent(
                    h => backButton.onPointerUp += h,
                    h => backButton.onPointerUp -= h)
                .Subscribe(_ => sceneService.PopScene())
                .AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}
