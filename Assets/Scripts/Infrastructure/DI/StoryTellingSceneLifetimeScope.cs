using DefaultNamespace;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class StoryTellingSceneLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<StoryTellingScenePresenter>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<GenericButton>();
    }
}
