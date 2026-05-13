using System;
using Features.Credit.Presentation.Interfaces;
using Infrastructure.Services;
using R3;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class CreditScenePresenter : IPostInitializable, IDisposable
    {
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly ICreditView creditView;

        private readonly CompositeDisposable _disposables = new();

        public void PostInitialize()
        {
            Observable.FromEvent(
                    h => creditView.BackButton.onPointerUp += h,
                    h => creditView.BackButton.onPointerUp -= h)
                .Subscribe(_ => sceneService.PopScene())
                .AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}
