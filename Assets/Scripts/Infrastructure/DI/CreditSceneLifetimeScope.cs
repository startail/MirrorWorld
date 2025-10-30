using System.Collections.Generic;
using DefaultNamespace;
using Features.Credit.Presentation.Interfaces;
using Features.Credit.Presentation.Views;
using Infrastructure.DI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class CreditSceneLifetimeScope : LifetimeScope
{
    [SerializeField] public List<CreditViewSet> creditViewSets;
    
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
        
        foreach (var creditViewSet in creditViewSets)
        {
            bool isCorrectPlatform = creditViewSet.platform == platformSettings?.GetPlatform;
            creditViewSet.creditView.gameObject.SetActive(isCorrectPlatform);
            if (isCorrectPlatform)
            {
                builder.RegisterInstance(creditViewSet.creditView).As<ICreditView>().AsSelf();
            }
        }

        // Register Presenter
        builder.RegisterEntryPoint<CreditScenePresenter>(Lifetime.Singleton);
    }
}
