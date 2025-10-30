using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GenericDroppableSource : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<GameObject> onDrop;
    public event Action onPointerEnter;
    public event Action onPointerExit;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            onDrop?.Invoke(eventData.pointerDrag);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
    }
}