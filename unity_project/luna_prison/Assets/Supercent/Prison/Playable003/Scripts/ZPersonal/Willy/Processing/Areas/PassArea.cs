using System;
using System.Collections;
using Supercent.Util;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class PassArea : MonoBehaviour
    {
        protected Action _onFunction;
        public event Action OnFunction { add { _onFunction += value; } remove { _onFunction -= value; } }

        [Header("Default")]
        [SerializeField] Collider _collider;
        [SerializeField] float _interval = 0.3f;

        [Header("Option")]
        [SerializeField] SpriteRenderer _footHold;
        [SerializeField] Color _inColor;
        Color _originColor;

        private Coroutine _coroutine;

        protected bool _isCameraMove = false;
        protected bool _canUse = false;
        protected bool _isEnter = false;
        protected bool _isAutoMod = false;
        private void Start()
        {
            _originColor = _footHold.color;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            _isEnter = true;
            _footHold.color = _inColor;
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            _isEnter = false;
            _footHold.color = _originColor;
        }
        protected virtual void Update()
        {
            if (_isCameraMove)
                return;
            if (!_canUse)
                return;

            if (_isEnter || _isAutoMod)
            {
                _onFunction?.Invoke();
            }
        }

        public virtual void WaitAndReuseArea()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(CoWaitAndReuseArea());
        }

        public void StopArea()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _canUse = false;
        }

        public void ReuseArea()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _canUse = true;
        }

        IEnumerator CoWaitAndReuseArea()
        {
            yield return CoroutineUtil.WaitForSeconds(_interval);
            _canUse = true;
            _coroutine = null;
        }

        public void Automation(bool on = true)
        {
            _isAutoMod = on;
            _collider.enabled = !on;

            if (_footHold == null)
                return;
            _footHold.enabled = !on;
        }

        public void CheckCameraMove(bool isMove)
        {
            _isCameraMove = isMove;
        }

        public void CompleteImmediately()
        {
            _onFunction?.Invoke();
        }
    }
}