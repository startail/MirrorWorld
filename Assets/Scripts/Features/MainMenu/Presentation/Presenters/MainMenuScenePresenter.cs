using System.Collections.Generic;
using AnnulusGames.SceneSystem;
using Features.MainMenu.Presentation.Interfaces;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class MainMenuScenePresenter : IPostInitializable
    {
        // Root
        [Inject] private readonly SceneService sceneService;
        
        // Scene
        [Inject] private readonly IMainMenuView mainMenuView;
        
        public void PostInitialize()
        {
            mainMenuView.BackButton.onPointerUp += () =>
            {
                if (sceneService.peekSceneKey != SceneKey.MainMenu) return;
                sceneService.PopScene();
            };
        }
    }
}