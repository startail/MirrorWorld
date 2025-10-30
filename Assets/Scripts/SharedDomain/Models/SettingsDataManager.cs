using System.Collections.Generic;
using AnnulusGames.SceneSystem;
using DefaultNamespace;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class SettingsDataManager
{
    public List<LanguageSet> languageSets;
    public LanguageSet currentLanguageSet;
    public float bgmVolume = 0.0f;
    public float seVolume = 0.0f;
    public bool isAutoPlay = false;
    
    [Inject]
    public SettingsDataManager(List<LanguageSet> languageSets)
    {
        this.languageSets = languageSets;
    }
    
    public void Initialize()
    {
        currentLanguageSet = GetCurrentLanguageSet();
        SetCurrentLanguageSet(currentLanguageSet);
        bgmVolume = 0.5f;
        seVolume = 0.5f;
        isAutoPlay = false;
    }

    public void ReloadSettings()
    {
        //ReloadLanguage();    
    }
    
    private void ReloadLanguage()
    {
        LocalizationManager.CurrentLanguageCode = currentLanguageSet.languageCode;
    }
    
    public LanguageSet GetCurrentLanguageSet()
    {
        foreach (LanguageSet languageSet in languageSets)
        {
            if( languageSet.languageCode == LocalizationManager.CurrentLanguage ) return languageSet;
        }
        return languageSets[0];
    }
    
    public void SetCurrentLanguageSet(LanguageSet languageSet)
    {
        currentLanguageSet = languageSet;
        LocalizationManager.CurrentLanguageCode = languageSet.languageCode;
    }
    
    [System.Serializable]
    public class LanguageSet
    {
        public string languageName;
        public string languageCode;
    }
}
