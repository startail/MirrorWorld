using System.Collections.Generic;
using AnnulusGames.SceneSystem;
using Features.Title.Presentation.Interfaces;
using Infrastructure.Services;
using SharedPresentation.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class TitleScenePresenter : IPostInitializable
    {
        // Root
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly ProgressDataManager progressDataManager;
        
        // Scene
        [Inject] private readonly ITitleView titleView;
        
        public void PostInitialize()
        {
            if( titleView is IGenericView view) view.Show();
            
            titleView.ToSettingsButton.onPointerUp += () =>
            {
                if (sceneService.peekSceneKey != SceneKey.Title) return;
                sceneService.PushScene(SceneKey.Settings, true);
            };
            titleView.ToCreditButton.onPointerUp += () =>
            {
                if (sceneService.peekSceneKey != SceneKey.Title) return;
                sceneService.PushScene(SceneKey.Credit, true);
            };
            titleView.ToMainMenuButton.onPointerUp += () =>
            {
                if (sceneService.peekSceneKey != SceneKey.Title) return;
                sceneService.PushScene(SceneKey.MainMenu, false);
            };
        }
    }
}