using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlatformSettings", menuName = "Scriptable Objects/WorldSettings")]
public class WorldSettings : ScriptableObject
{
    [SerializeField] public Vector2Int worldResolution = new Vector2Int(1920, 1080);
    [SerializeField] public Vector2 worldAspectRatio = new Vector2(16, 9);
    [SerializeField] public float worldPPU = 100f;
}
