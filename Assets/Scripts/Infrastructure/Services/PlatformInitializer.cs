using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

// PlatformInitializer.cs (外部スクリプト)
public class PlatformInitializer : IStartable
{
    private readonly WorldSettings worldSettings;
    private readonly PlatformSettings currentPlatformSettings; // DIで受け取る

    [Inject]
    public PlatformInitializer(WorldSettings worldSettings, PlatformSettings currentPlatformSettings)
    {
        // DIコンテナから特定済みの PlatformSettings インスタンスを受け取る
        this.worldSettings = worldSettings;
        this.currentPlatformSettings = currentPlatformSettings;

        // 初期化処理の一部（向き設定など）
        SetScreenOrientation();
    }
    
    public void Start()
    {
        float targetHeight = currentPlatformSettings.alterOrientationFromWorld ? worldSettings.worldResolution.x : worldSettings.worldResolution.y;
        float deviceSafeAreaRatio = Screen.height / Screen.safeArea.height;
        float orthographicSize = targetHeight / deviceSafeAreaRatio / worldSettings.worldPPU / 2.0f;
        foreach (var camera in GameObject.FindGameObjectsWithTag("MainCamera"))
        {
            camera.GetComponent<Camera>().orthographicSize = orthographicSize;
        }
        foreach (var camera in GameObject.FindGameObjectsWithTag("MainCanvas"))
        {
            camera.GetComponent<CanvasScaler>().uiScaleMode = currentPlatformSettings.uiScaleMode;
            camera.GetComponent<CanvasScaler>().referenceResolution = currentPlatformSettings.referenceResolution;
            camera.GetComponent<CanvasScaler>().referencePixelsPerUnit = worldSettings.worldPPU;
        }
    }
    
    public PlatformSettings GetCurrentPlatformSettings() => currentPlatformSettings;
    
    void SetScreenOrientation()
    {
#if UNITY_ANDROID
        // Android特有の向き設定を適用
        Screen.orientation = currentPlatformSettings.defaultOrientation;
#elif UNITY_STANDALONE_WIN
    // Windows向けの設定 (向きは通常固定かユーザー制御)
    // 必要に応じてウィンドウサイズの初期設定などを行う
#elif UNITY_WEBGL
    // WebGL向けの設定 (通常は特別な向き設定は不要)
#elif UNITY_EDITOR
                
#endif
    }
}
