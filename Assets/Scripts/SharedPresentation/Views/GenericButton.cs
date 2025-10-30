using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Button))]
    public class GenericButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler
    {
        [Header("GenericButton Properties")]
        [SerializeField] public Image background;
        [SerializeField] public TextMeshProUGUI tmp;
        [SerializeField] public Image contents;
        public event Action onPointerUp;
        public event Action onPointerDown;
        public event Action onPointerEnter;
        public event Action onPointerExit;
        public event Action onClick;
        public event Action onLeftClick;
        public event Action onRightClick;
        public event Action onLongPress;
        
        private float longPressDuration = 2f;
        private float countUpTimer = 0.0f;
        private bool longPressChallenge = false;
        public virtual void OnLongPress(PointerEventData eventData)
        {
            onLongPress?.Invoke();
        }
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke();
            if( longPressChallenge && countUpTimer >= longPressDuration )
            {
                OnLongPress(eventData);
            }
            longPressChallenge = false;
            countUpTimer = 0.0f;
        }
        
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke();
            countUpTimer = 0.0f;
            longPressChallenge = true;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke();
        }
    
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onLeftClick?.Invoke();
            }
            else
            {
                onRightClick?.Invoke();
            }
        }

        void Update()
        {
            if( !longPressChallenge ) return;
            countUpTimer += Time.deltaTime;
        }
    }
}