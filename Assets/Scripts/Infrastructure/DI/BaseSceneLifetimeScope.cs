using DefaultNamespace;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class BaseSceneLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<BaseScenePresenter>();
    }
}
