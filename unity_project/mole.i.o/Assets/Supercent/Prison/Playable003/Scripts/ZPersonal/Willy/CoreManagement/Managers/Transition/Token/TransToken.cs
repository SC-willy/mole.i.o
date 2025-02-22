using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class TransitionToken
    {
        public Transform Target => _target;
        public Transform Start => _startTransform;
        public Transform End => _endTransform;
        public float Time => _clampedTime;
        public bool IsCancled => _isCancled;


        protected TransitionTween _tweenData;
        protected Transform _target;
        protected Transform _startTransform;
        protected Transform _endTransform;
        private Action _callBack;

        private float _elapsedTime;
        private float _clampedTime;
        private bool _isLooping;
        private bool _isCancled;

        public void Initialize(TransitionTween data, Transform target, Transform start, Transform end, Action doneCallback = null)
        {
            _tweenData = data;
            _target = target;
            _startTransform = start;
            _endTransform = end;
            _elapsedTime = 0f;
            _callBack = null;
            _callBack = doneCallback;
            _isLooping = _tweenData.Loop;
            _isCancled = false;
            _Initialize();
        }

        protected virtual void _Initialize() { }

        public bool UpdateTransition(float deltaTime)
        {
            if (_target == null) return false;

            _elapsedTime += deltaTime;
            _clampedTime = Mathf.Clamp01(_elapsedTime * _tweenData.InversedDuration);

            _tweenData.UpdateTransition(this);

            if (_isLooping && _clampedTime >= 1f)
            {
                _elapsedTime = 0f;
                return true;
            }

            return _clampedTime < 1f;
        }

        public void Reset()
        {
            _tweenData = null;
            _target = null;
            _startTransform = null;
            _endTransform = null;
            _elapsedTime = 0f;

            if (!_isCancled)
                _callBack?.Invoke();
        }

        public void Stop()
        {
            _target = null;
            _isLooping = false;
            _isCancled = true;
        }
    }
}