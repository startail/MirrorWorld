using System.Collections.Generic;
using DefaultNamespace;
using Features.Title.Presentation.Interfaces;
using Features.Title.Presentation.Views;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.DI
{
    public class TitleSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] public List<TitleViewSet> titleViewSets;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register Model

            // Register View
            PlatformSettings platformSettings = null;
            LifetimeScope[] allScopes = Object.FindObjectsByType<LifetimeScope>(FindObjectsSortMode.None);
            foreach (LifetimeScope scope in allScopes)
            {
                if (scope is RootLifetimeScope) platformSettings = scope.Container.Resolve<PlatformSettings>();
            }

            foreach (var titleViewSet in titleViewSets)
            {
                bool isCorrectPlatform = titleViewSet.platform == platformSettings?.GetPlatform;
                titleViewSet.titleView.gameObject.SetActive(isCorrectPlatform);
                if (isCorrectPlatform) builder.RegisterInstance(titleViewSet.titleView).As<ITitleView>().AsSelf();
            }

            // Register Presenter
            builder.RegisterEntryPoint<TitleScenePresenter>(Lifetime.Singleton);
        }
    }
}