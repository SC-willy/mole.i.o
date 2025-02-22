using System;
using Supercent.Util;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class BoolTrigger : MonoBehaviour
    {
        protected Action _onTriggerEnterEvent;
        public event Action OnTriggerEnterEvent { add { _onTriggerEnterEvent += value; } remove { _onTriggerEnterEvent -= value; } }
        [SerializeField] protected LayerMask _layer;
        protected bool _isEnter = false;

        protected virtual void OnTriggerEnter(Collider other)
        {
            _isEnter = _layer.HasLayer(other.gameObject.layer);
            if (_isEnter)
                _onTriggerEnterEvent?.Invoke();
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (_isEnter && _layer.HasLayer(other.gameObject.layer))
                _isEnter = false;
        }

        public void SetOnEnter(Action action, bool _isRegist)
        {
            if (_isRegist)
                OnTriggerEnterEvent += action;
            else
                OnTriggerEnterEvent -= action;
        }
    }
}