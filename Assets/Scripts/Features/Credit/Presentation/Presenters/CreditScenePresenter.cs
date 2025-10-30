using AnnulusGames.SceneSystem;
using Features.Credit.Presentation.Interfaces;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class CreditScenePresenter : IPostInitializable
    {
        // Root
        [Inject] private readonly SceneService sceneService;
        
        // Scene
        [Inject] private readonly ICreditView creditView;
        
        public void PostInitialize()
        {
            creditView.BackButton.onPointerUp += () => sceneService.PopScene();
        }
    }
}