using System;
using UnityEngine;
using UnityEngine.Events;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class ActionRegistEvent : UnityEvent<Action, bool> { }
    [Serializable]
    public class FlowCheckEvent : UnityEvent<Action<bool>> { }

    [Serializable]
    public class Flow
    {
        [SerializeField] string _label = string.Empty;
        [SerializeField] string _logName = string.Empty;

        [Space]
        [SerializeField] ActionRegistEvent _registEvent = null;
        [SerializeField] FlowCheckEvent _endCheckEvent = null;
        [SerializeField] UnityEvent _endEvent = null;
        [SerializeField] public int[] NextIndex = null;

        event Action<Flow, int[]> _onCallNextFlow = null;
        bool _isPassable = false;

        public void StartFlow() => _registEvent?.Invoke(EndFlow, true);
        public void SetFlowCallMethod(Action<Flow, int[]> action) => _onCallNextFlow = action;
        void GetCanPassFlow(bool isPassable) => _isPassable = isPassable;

        public void StopFlow()
        {
            _registEvent?.Invoke(EndFlow, false);
        }
        private void CheckCanPassFlow()
        {
            if (_endCheckEvent.GetPersistentEventCount() <= 0)
                _isPassable = true;
            else
                _endCheckEvent?.Invoke(GetCanPassFlow);
        }
        public void EndFlow()
        {
            CheckCanPassFlow();
            if (!_isPassable)
                return;

            _onCallNextFlow?.Invoke(this, NextIndex);

            _endEvent?.Invoke();
            _registEvent?.Invoke(EndFlow, false);
        }
    }
}
