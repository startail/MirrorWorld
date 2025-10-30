using DefaultNamespace;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class IngameSceneLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<IngameScenePresenter>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<GenericButton>();
    }
}
