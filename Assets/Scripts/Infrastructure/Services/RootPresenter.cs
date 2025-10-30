using AnnulusGames.SceneSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Services
{
    public class RootPresenter : IInitializable
    {
        [Inject] ProgressDataManager progressDataManager;
        [Inject] SettingsDataManager settingsDataManager;
        
        public void Initialize()
        {
            // 各種DataManagerデータのロード。データがなければ初期化
            progressDataManager.Initialize();
            settingsDataManager.Initialize();
        }
    }
}