using AnnulusGames.SceneSystem;
using Features.Settings.Presentation.Interfaces;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Features.Settings.Presentation.Presenters
{
    public class SettingsScenePresenter : IPostInitializable
    {
        // Root
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly AudioService audioService;
        [Inject] private readonly SettingsDataManager settingsDataManager;
        
        // Scene
        [Inject] private readonly ISettingsView settingsView;
        
        public void PostInitialize()
        {
            /*
            languageButton.SetText(settingsDataManager.currentLanguageSet.languageName);
            languageButton.onPointerUp.AddListener(() =>
            {
                ChangeToNextLanguage();
            });
            */
            
            settingsView.BackButton.onPointerUp += () =>
            {
                sceneService.PopScene();
            };
        }
        
        public void ChangeToNextLanguage()
        {
            for(int index=0; index<settingsDataManager.languageSets.Count; index++)
            {
                if (settingsDataManager.languageSets[index].languageCode == settingsDataManager.currentLanguageSet.languageCode)
                {
                    if (index == settingsDataManager.languageSets.Count - 1)
                    {
                        settingsDataManager.SetCurrentLanguageSet( settingsDataManager.languageSets[0] );
                    }
                    else
                    {
                        settingsDataManager.SetCurrentLanguageSet( settingsDataManager.languageSets[index+1] );
                    }
                    //languageButton.SetText(settingsDataManager.currentLanguageSet.languageName);
                    break;
                }
            }
        }
    }
}