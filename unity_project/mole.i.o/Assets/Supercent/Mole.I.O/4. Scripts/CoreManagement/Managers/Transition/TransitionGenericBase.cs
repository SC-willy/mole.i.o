using System;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public abstract class TransitionGenericBase<T> : TransitionTween where T : TransitionToken, new()
    {

        [SerializeField] protected TransitionTween[] _linkedTransition;
        Stack<T> _tweenPool = new Stack<T>();
        protected List<T> _activeTweens = new List<T>();
        protected Dictionary<Transform, T> _transitionData = new Dictionary<Transform, T>();


        public override void StartSetup()
        {
            _inversedDuration = 1f / _duration;
            _tweenPool = new Stack<T>(_poolCount);
        }


        public override void StartTransition(Transform target, Transform start, Transform end, Action doneCallback = null)
        {
            if (!_unstopable)
                OnOverwriteUpdate?.Invoke(target, this);
            T token = GetToken();
            token.Initialize(this, target, start, end, doneCallback);
            _activeTweens.Add(token);

            if (!_transitionData.ContainsKey(target))
            {
                _transitionData.Add(target, token);
            }
            else
            {
                _transitionData[target] = token;
            }
        }
        public override void UpdateManualy(float deltaTime)
        {
            for (int i = 0; i < _activeTweens.Count; i++)
            {
                T curToken = _activeTweens[i];
                if (!curToken.UpdateTransition(deltaTime))
                {
                    ReturnToPool(curToken);
                    _activeTweens.Remove(curToken);
                    i--;
                }
            }
        }
        public abstract void _UpdateTransition(T token);
        public void UpdateTransitionGeneric(T token)
        {
            for (int j = 0; j < _linkedTransition.Length; j++)
            {
                _linkedTransition[j].UpdateLinkTransition(token);
            }
            _UpdateTransition(token);
        }
        public override void UpdateTransition(TransitionToken token) => UpdateTransitionGeneric(token as T);
        public override void UpdateLinkTransition(TransitionToken token) => _UpdateTransition(token as T);

        protected T GetToken()
        {
            if (_tweenPool.Count > 0)
            {
                return _tweenPool.Pop();
            }
            return new T();
        }

        private void ReturnToPool(T tween)
        {
            tween.Reset();
            _tweenPool.Push(tween);
        }

        public override void StopTransition(Transform target)
        {
            _transitionData[target].Stop();
        }
    }
}