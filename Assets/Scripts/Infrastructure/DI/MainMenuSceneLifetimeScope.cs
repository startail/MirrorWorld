using System.Collections.Generic;
using DefaultNamespace;
using Features.MainMenu.Presentation.Interfaces;
using Features.MainMenu.Presentation.Views;
using Infrastructure.DI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class MainMenuSceneLifetimeScope : LifetimeScope
{
    [SerializeField] public List<MainMenuViewSet> mainMenuViewSets;
    
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

        foreach (var mainMenuViewSet in mainMenuViewSets)
        {
            bool isCorrectPlatform = mainMenuViewSet.platform == platformSettings?.GetPlatform;
            mainMenuViewSet.mainMenuView.gameObject.SetActive(isCorrectPlatform);
            if (isCorrectPlatform) builder.RegisterInstance(mainMenuViewSet.mainMenuView).As<IMainMenuView>().AsSelf();
        }

        // Register Presenter
        builder.RegisterEntryPoint<MainMenuScenePresenter>(Lifetime.Singleton);
    }
}
