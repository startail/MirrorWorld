using AnnulusGames.SceneSystem;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class BaseScenePresenter : IStartable, ITickable
    {
        [Inject] private SceneService sceneService;
        //[Inject] MWScenesAccesser scenesAccesser;
        //[Inject] MWScenesManager scenesManager;
        
        public void Start()
        {
            Debug.Log(sceneService.peekSceneKey);
            sceneService.PushScene(sceneService.peekSceneKey, false , true);
            
            /*
            scenesAccesser.afterCreateContainer.RemoveAllListeners();
            scenesAccesser.afterCreateContainer.AddListener(() =>
            {
                scenesAccesser.PushScene(scenesManager.peekSceneKey);
            });
            */
        }

        public void Tick()
        {
            
        }
    }
}