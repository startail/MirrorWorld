using AnnulusGames.SceneSystem;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class StoryTellingScenePresenter : IPostInitializable
    {
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly GenericButton backButton;
        
        public void PostInitialize()
        {
            backButton.onPointerUp += () =>
            {
                sceneService.PopScene();
            };
        }
    }
}