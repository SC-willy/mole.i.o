using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Supercent.PrisonLife.Playable003
{
    public class ElevatorTouchChecker : MonoBehaviour, IPointerDownHandler
    {
        public event Action OnPointerDownEvent;
        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownEvent?.Invoke();
        }
    }
}