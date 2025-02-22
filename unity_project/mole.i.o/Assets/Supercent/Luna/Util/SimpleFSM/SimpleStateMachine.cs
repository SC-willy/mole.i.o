using System.Collections.Generic;
using UnityEngine;

namespace Supercent.Util.SimpleFSM
{
    public class SimpleStateMachine
    {
        //------------------------------------------------------------------------------
        // variables
        //------------------------------------------------------------------------------
        protected bool _isInited = false;

        protected MonoBehaviour _coroutineOwner = null;
        protected readonly Dictionary<string, SimpleState> _stateSet = new Dictionary<string, SimpleState>();
        protected SimpleState _currentState = null;

        //------------------------------------------------------------------------------
        // get, set
        //------------------------------------------------------------------------------
        public bool IsInited => _isInited;

        public virtual SimpleState CurrentState => _currentState;
        public virtual string CurrentStateKey =>_currentState == null ? string.Empty : _currentState.StateKey;

        //------------------------------------------------------------------------------
        // functions
        //------------------------------------------------------------------------------
        public virtual void Init(MonoBehaviour coroutineOwner,
                                 object initParam,
                                 params SimpleState[] states)
        {
            Release();

            if (coroutineOwner == null)
            {
                Debug.LogError($"{nameof(Init)} : coroutineOwner 가 없습니다. 상태 내부에서 코루틴을 사용하지 못할 수 있습니다.");
                return;
            }

            if (states != null)
            {
                for (int index = 0; index < states.Length; ++index)
                {
                    var state = states[index];
                    if (state == null)
                        continue;

                    if (string.IsNullOrEmpty(state.StateKey))
                    {
                        Debug.LogError($"{nameof(Init)} : Key 값이 Null 또는 Empty 입니다.");
                        continue;
                    }

#if UNITY_EDITOR
                    if (_stateSet.ContainsKey(state.StateKey))
                        Debug.LogError($"{nameof(Init)} : {state.StateKey}는 이미 존재하는 상태 입니다.");
#endif// UNITY_EDITOR
                    _stateSet[state.StateKey] = state;
                    state.Init(coroutineOwner, ChangeState, initParam);
                }
            }

            if (_stateSet.Count < 1)
            {
                Debug.LogError($"{nameof(Init)} : 사용할 상태가 하나도 없습니다.");
                return;
            }
        }

        public virtual void Update()
        {
            if (_currentState != null)
                _currentState.Update();
        }

        public virtual void Release()
        {
            _isInited = false;
            _currentState = null;
            _coroutineOwner = null;

            if (_stateSet != null)
            {
                foreach (var state in _stateSet.Values)
                    state.Release();
                _stateSet.Clear();
            }
        }

        public virtual void ChangeState(string key, object param = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError($"[SimpleFSM.SimpleStateMachine.ChangeState] 키값이 Null 또는 Empty 입니다.");
                return;
            }

            if (null == _stateSet)
            {
                Debug.LogError("[SimpleFSM.SimpleStateMachine.ChangeState] 사용할 상태가 하나도 없습니다.");
                return;
            }

            if (!_stateSet.TryGetValue(key, out var state))
            {
                Debug.LogError($"[SimpleFSM.SimpleStateMachine.ChangeState] 변경할 상태를 찾을 수 없습니다. 변경할 상태: {key.ToString()}");
                return;
            }

            if (null == state)
            {
                Debug.LogError($"[SimpleFSM.SimpleStateMachine.ChangeState] 변경할 상태가 정상이 아닙니다. 변경할 상태: {key.ToString()}");
                return;
            }

            string preStateKey;
            if (_currentState == null)
                preStateKey = string.Empty;
            else
            {
                preStateKey = _currentState.StateKey;
                _currentState.Finish(state.StateKey);
            }

            _currentState = state;
            _currentState.Start(preStateKey, param);
        }
    }
}