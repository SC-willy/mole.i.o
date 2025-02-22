using System;
using System.Collections;
using UnityEngine;

namespace Supercent.Util.SimpleFSM
{
    public abstract class SimpleState
    {
        //------------------------------------------------------------------------------
        // variables
        //------------------------------------------------------------------------------
        MonoBehaviour _coroutineOwner = null;
        Action<string, object> _onChangeState = null;

        //------------------------------------------------------------------------------
        // get, set
        //------------------------------------------------------------------------------
        public abstract string StateKey { get; }

        //------------------------------------------------------------------------------
        // functions
        //------------------------------------------------------------------------------
        public void Init(MonoBehaviour coroutineOwner, Action<string, object> onChangeState, object initParam = null)
        {
            Release();

            _coroutineOwner = coroutineOwner;
            _onChangeState = onChangeState;

            _Init(initParam);
        }

        public void Release()
        {
            _coroutineOwner = null;
            _onChangeState = null;

            _Release();
        }

        public void Start(string preStateKey, object startParam = null) => _Start(preStateKey, startParam);
        public void Finish(string nextStateKey) => _Finish(nextStateKey);
        public void Update() => _Update();

        protected virtual void _Init(object initParam) { }
        protected virtual void _Release() { }
        protected virtual void _Start(string preStateKey, object startParam) { }
        protected virtual void _Finish(string nextStateKey) { }
        protected virtual void _Update() { }



        protected void ChangeState(string key, object param = null)
        {
            if (_onChangeState != null)
                _onChangeState(key, param);
        }


        protected Coroutine StartCoroutine(IEnumerator func)
        {
            if (_coroutineOwner == null)
            {
                Debug.LogError($"{nameof(StartCoroutine)} : 코루틴을 사용할 수 있는 상태가 아닙니다.");
                return null;
            }

            return func == null ? null : _coroutineOwner.StartCoroutine(func);
        }

        protected void SwapCoroutine(ref Coroutine target, IEnumerator func)
        {
            if (_coroutineOwner == null)
            {
                Debug.LogError($"{nameof(SwapCoroutine)} : 코루틴을 사용할 수 있는 상태가 아닙니다.");
                return;
            }

            if (target != null)
                _coroutineOwner.StopCoroutine(target);
            target = func == null ? null : _coroutineOwner.StartCoroutine(func);
        }

        protected void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null && _coroutineOwner != null)
                _coroutineOwner.StopCoroutine(coroutine);
        }

        protected void ReleaseCoroutine(ref Coroutine coroutine)
        {
            if (_coroutineOwner != null)
            {
                if (coroutine != null)
                    _coroutineOwner.StopCoroutine(coroutine);
            }
            coroutine = null;
        }
    }
}