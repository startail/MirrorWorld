using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GenericGrabbable : MonoBehaviour, IPointerMoveHandler , IPointerUpHandler, IPointerDownHandler
{
    [Header("GenericGrabbable Properties")]
    [SerializeField, Range(0,1)] public float stickeyness = 0.0f;
    [SerializeField] public GameObject linkageTarget;
    [SerializeField] public Camera focusCamera;
    public event Action onGrabStart;
    public event Action onGrabEnd;
    private Vector3 _startPosition;
    private Vector3 _linkageTargetStartPosition;
    private bool _grabbing = false;
    private Vector3 beforePosition = new Vector3();
    
    public bool Grabbing
    {
        get => _grabbing;
        set
        {
            _grabbing = value;
            if (_grabbing)
            {
                onGrabStart?.Invoke();
            }
            else
            {
                onGrabEnd?.Invoke();
            }
        }
    }
        
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Grabbing = false;
    }
    
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Grabbing = true;
        
        if (gameObject.GetComponent<Transform>())
        {
            Camera currentCamera = focusCamera!=null?focusCamera:Camera.main;
            _startPosition = currentCamera.ScreenToWorldPoint(eventData.position);
            if (linkageTarget != null)
            {
                _linkageTargetStartPosition = linkageTarget.GetComponent<Transform>().position;
            }
            else
            {
                _linkageTargetStartPosition = gameObject.GetComponent<Transform>().position;
            }
        }
    }
    
    public void OnPointerMove(PointerEventData eventData)
    {
        if( Grabbing )
        {
            Camera currentCamera = focusCamera!=null?focusCamera:Camera.main;
            MoveTarget(currentCamera.ScreenToWorldPoint(eventData.position) );
        }
    }

    public void Update()
    {
        if (Grabbing && stickeyness > 0)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            float screenLongerSide = UnityEngine.Device.Screen.width > UnityEngine.Device.Screen.height ? UnityEngine.Device.Screen.width : UnityEngine.Device.Screen.height;
            float totalStickeyness = stickeyness * screenLongerSide;
            if( ( mousePosition - beforePosition ).magnitude < totalStickeyness )
            {
                MoveTarget(mousePosition);    
            }
            beforePosition = mousePosition;
        }
    }
    
    private void MoveTarget(Vector3 nextWorldPos)
    {
        if (linkageTarget != null)
        {
            if (gameObject.GetComponent<RectTransform>())
            {
                //
            }
            else if (linkageTarget.GetComponent<Transform>())
            {
                transform.position = nextWorldPos;
            }   
        }
        else
        {
            if (gameObject.GetComponent<RectTransform>())
            {
                // WorldToScreenPointの処理を挟んでUIを動かす
            }
            else if (gameObject.GetComponent<Transform>())
            {
                linkageTarget.GetComponent<Transform>().position = nextWorldPos;
            }
        }
    }
}