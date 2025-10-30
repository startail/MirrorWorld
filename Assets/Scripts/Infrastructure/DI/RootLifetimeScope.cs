using System.Collections.Generic;
using DefaultNamespace;
using Infrastructure.Services;
using SharedDomain.Repositories;
using VContainer.Unity;
using UnityEngine;
using VContainer;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Infrastructure.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        // For Platform and World Settings
        [SerializeField] public WorldSettings worldSettings;
        [SerializeField] public List<PlatformSettings> platformSettingsList;
        // For Scene Management
        [SerializeField] public MainLoadingScreen mwloadingScreenPrefab;
        [SerializeField] private SceneController sceneControllerPrefab;
        
        [SerializeField] public List<SettingsDataManager.LanguageSet> languageSets;
        

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(worldSettings).AsSelf();
            builder.RegisterInstance(platformSettingsList).AsSelf();
            PlatformSettings currentPlatformSettings = DeterminePlatformSettings(worldSettings, platformSettingsList);
            builder.RegisterInstance(currentPlatformSettings).AsSelf().As<IPlatform>();
            builder.RegisterEntryPoint<PlatformInitializer>(Lifetime.Transient); // Scene毎に稼働してもらう関係でTransient
            builder.Register<AudioService>(Lifetime.Singleton);
            builder.Register<PlayerPrefsProgressRepository>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ProgressDataManager>(Lifetime.Singleton);
            builder.Register<SettingsDataManager>(Lifetime.Singleton)
                .WithParameter<List<SettingsDataManager.LanguageSet>>(languageSets);
            builder.RegisterEntryPoint<RootPresenter>();

            // for Scene Management
            builder.RegisterComponentOnNewGameObject<SceneController>(Lifetime.Singleton, "SceneController")
                .DontDestroyOnLoad()
                .AsSelf();
            builder.Register<SceneService>(Lifetime.Singleton)
                .WithParameter<MainLoadingScreen>(mwloadingScreenPrefab)
                .AsSelf()
                .AsImplementedInterfaces();
        }
        
        private PlatformSettings DeterminePlatformSettings(WorldSettings worldSettings, List<PlatformSettings> platformSettingsList)
        {
            RuntimePlatform currentPlatform = Application.platform;
            PlatformSettings determinedSettings = null;
            
            foreach (var settings in platformSettingsList)
            {
                if (settings.targetPlatform == currentPlatform)
                {
                    determinedSettings = settings;
                    break;
                }
            }

            if (determinedSettings == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("No matching PlatformSettings found. Using Editor Build Settings.");
                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                RuntimePlatform editorPlatform;

                switch (target)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        editorPlatform = RuntimePlatform.WindowsPlayer;
                        break;
                    case BuildTarget.Android:
                        editorPlatform = RuntimePlatform.Android;
                        break;
                    case BuildTarget.iOS:
                        editorPlatform = RuntimePlatform.IPhonePlayer;
                        break;
                    case BuildTarget.WebGL:
                        editorPlatform = RuntimePlatform.WebGLPlayer;
                        break;
                    case BuildTarget.StandaloneOSX:
                        editorPlatform = RuntimePlatform.OSXPlayer;
                        break;
                    default:
                        Debug.LogWarning($"Unknown BuildTarget: {target}. Falling back to WindowsPlayer.");
                        editorPlatform = RuntimePlatform.WindowsPlayer;
                        break;
                }
                
                Debug.LogWarning($"BuildTarget is: {target}.");
                
                foreach (var settings in platformSettingsList)
                {
                    if (settings.targetPlatform == editorPlatform)
                    {
                        determinedSettings = settings;
                        break;
                    }
                }
#else 
                Debug.LogWarning($"No matching PlatformSettings found. Using {platformSettingsList[0].targetPlatform} Settings.");
                determinedSettings = platformSettingsList[0];
#endif                
            }

            return determinedSettings;
        }
    }
}