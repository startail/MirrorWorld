using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GenericCheckBox : GenericButton
{
    [Header("GenericCheckBox Properties")]
    [SerializeField] public Image checkMark;
    [SerializeField] public bool activateEvent;
    public event Action onCheckOff;
    public event Action onCheckOn;

    private bool _check;
    public bool Check
    {
        get => _check;
        set
        {
            _check = value;
            if (_check)
            {
                ShowCheckMark();
                if(activateEvent) onCheckOn?.Invoke();
            }
            else
            {
                HideCheckMark();
                if(activateEvent) onCheckOff?.Invoke();
            }
        }
    }
    
    public virtual void ShowCheckMark()
    {
        if( contents != null ) contents.gameObject.SetActive(false);
        if( checkMark != null ) checkMark.gameObject.SetActive(true);
    }
        
    public virtual void HideCheckMark()
    {
        if( contents != null ) contents.gameObject.SetActive(true);
        if( checkMark != null ) checkMark.gameObject.SetActive(false);
    }
        
    public override void OnPointerUp(PointerEventData eventData)
    {
        Check = !Check;
        if( activateEvent ) base.OnPointerUp(eventData);
    }
}