using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SafeAreaAdjuster : MonoBehaviour
    {
        private void Awake()
        {
            var panel = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;
            
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
        }
    }
}