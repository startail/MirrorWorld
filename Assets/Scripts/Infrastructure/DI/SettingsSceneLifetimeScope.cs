using System.Collections.Generic;
using DefaultNamespace;
using Features.Settings.Presentation.Interfaces;
using Features.Settings.Presentation.Presenters;
using Features.Settings.Presentation.Views;
using Infrastructure.DI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class SettingsSceneLifetimeScope : LifetimeScope
{
    [SerializeField] public List<SettingsViewSet> settingsViewSets;
    
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

        foreach (var settingsViewSet in settingsViewSets)
        {
            bool isCorrectPlatform = settingsViewSet.platform == platformSettings?.GetPlatform;
            settingsViewSet.settingsView.gameObject.SetActive(isCorrectPlatform);
            if (isCorrectPlatform)builder.RegisterInstance(settingsViewSet.settingsView).As<ISettingsView>().AsSelf();
        }

        // Register Presenter
        builder.RegisterEntryPoint<SettingsScenePresenter>(Lifetime.Singleton);
        
    }
}
