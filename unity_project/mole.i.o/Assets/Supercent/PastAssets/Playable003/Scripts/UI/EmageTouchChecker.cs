using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Supercent.MoleIO.InGame
{
    public class ImageTouchChecker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnPointerDownEvent;
        public event Action OnPointerUpEvent;
        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpEvent?.Invoke();
        }
    }
}