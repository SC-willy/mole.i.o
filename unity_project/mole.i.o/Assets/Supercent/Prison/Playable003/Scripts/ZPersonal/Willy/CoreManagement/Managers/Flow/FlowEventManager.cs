
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class FlowEventManager : IBindable
    {
        [SerializeField] List<Flow> _flow = new List<Flow>();

        public void StartFlow(int index = 0)
        {
            if (index >= _flow.Count)
                return;

            for (int i = 0; i < _flow.Count; i++)
            {
                _flow[i].SetFlowCallMethod(CallNextFlow);
            }

            _flow[index].StartFlow();
        }

        private void CallNextFlow(Flow flow, int[] nextIndex)
        {
            for (int i = 0; i < nextIndex.Length; i++)
            {
                if (nextIndex[i] >= _flow.Count)
                    break;
                _flow[nextIndex[i]].StartFlow();
            }
        }

        public void StopAllFlow()
        {
            for (int i = 0; i < _flow.Count; i++)
            {
                _flow[i].StopFlow();
            }
        }

#if UNITY_EDITOR
        [SerializeField] bool _isOneWay;
        [SerializeField] int _flowCount;
        public void Bind(MonoBehaviour mono)
        {
            if (_flow.Count < _flowCount)
            {
                int count = _flowCount - _flow.Count;
                for (int i = 0; i < count; i++)
                {
                    _flow.Add(new Flow());
                }
            }
            if (!_isOneWay)
                return;

            if (_flow.Count <= 1)
                return;

            for (int i = 0; i < _flow.Count - 1; i++)
            {
                _flow[i].NextIndex = new int[1] { i + 1 };
            }
        }
#endif // UNITY_EDITOR
    }
}