using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SettingsLanguageButton : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] private TextMeshProUGUI tmp;
        public UnityEvent onPointerUp;
        
        public void SetText(string text)
        {
            tmp.text = text;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp.Invoke();
        }
    }
}