using System;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class TransitionManager : IUpdateable, IBindable, IStartable
    {
        [SerializeField] TransitionTween[] _allTransitionData;
        Dictionary<Transform, TransitionTween> _updateInfo = new Dictionary<Transform, TransitionTween>();

        public void StartSetup()
        {
            TransitionTween _curTransition;
            for (int i = 0; i < _allTransitionData.Length; i++)
            {
                _curTransition = _allTransitionData[i];
                _curTransition.StartSetup();
                _curTransition.OnOverwriteUpdate += ReleasePreviousMotion;
            }
            _curTransition = null;
        }
        public void UpdateManualy(float dt)
        {
            for (int i = 0; i < _allTransitionData.Length; i++)
            {
                _allTransitionData[i].UpdateManualy(dt);
            }
        }

        private void ReleasePreviousMotion(Transform target, TransitionTween transitionInfo)
        {
            if (_updateInfo.ContainsKey(target))
            {
                _updateInfo[target].StopTransition(target);
                _updateInfo[target] = transitionInfo;
            }
            else
            {
                _updateInfo.Add(target, transitionInfo);
            }
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _allTransitionData = Resources.FindObjectsOfTypeAll<TransitionTween>();
        }

#endif //UNITY_EDITOR
    }

}
