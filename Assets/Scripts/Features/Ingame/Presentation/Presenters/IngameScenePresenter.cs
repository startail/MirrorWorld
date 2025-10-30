using Infrastructure.Services;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class IngameScenePresenter : IPostInitializable
    {
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly GenericButton backButton;
        
        public void PostInitialize()
        {
            backButton.onPointerUp += () =>
            {
                if (sceneService.peekSceneKey != SceneKey.Ingame) return;
                sceneService.PopScene();
            };
        }
    }
}