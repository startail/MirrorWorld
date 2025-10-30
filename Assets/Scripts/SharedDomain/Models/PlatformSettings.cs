using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlatformSettings", menuName = "Scriptable Objects/PlatformSettings")]
public class PlatformSettings : ScriptableObject, IPlatform
{
    // アスペクト比、向き、初期解像度、UIスケール
    // targetAspectRatio, defaultOrientation, resolutionPolicy
    [Header("Special Settings")]
    [SerializeField] public bool alterOrientationFromWorld = false;
    
    [Header("Platform Settings")]
    [SerializeField] public RuntimePlatform targetPlatform = RuntimePlatform.WindowsPlayer;
    [SerializeField] public Vector2 targetAspectRatio = new Vector2(16, 9);
    [SerializeField] public ScreenOrientation defaultOrientation = ScreenOrientation.LandscapeLeft;
    [SerializeField] public Vector2Int initialResolution = new Vector2Int(1920, 1080);
    [SerializeField] public FullScreenMode fullScreenMode = FullScreenMode.FullScreenWindow;
    
    [Header("Canvas Scaler Settings")]
    [SerializeField] public CanvasScaler.ScaleMode uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    [SerializeField] public Vector2Int referenceResolution = new Vector2Int(1920, 1080);
    [SerializeField] public CanvasScaler.ScreenMatchMode uiMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    [SerializeField, Range(0f, 1f)] public float matchWidthOrHeight = 0.5f;
    [SerializeField] public int referencePixelPerUnit = 1;
    
    public RuntimePlatform GetPlatform => targetPlatform;
}

public interface IPlatform
{
    RuntimePlatform GetPlatform { get; }
}