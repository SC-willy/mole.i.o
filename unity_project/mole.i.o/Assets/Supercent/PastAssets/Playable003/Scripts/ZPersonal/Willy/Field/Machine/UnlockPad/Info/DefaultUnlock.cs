using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class DefaultUnlock
    {
        [SerializeField] string _eventName = string.Empty;
        [SerializeField] GameObject[] _enableObjects;
        [SerializeField] GameObject[] _disableObjects;
        [SerializeField] int _code;
        [SerializeField] int _cost;
        public int Code => _code;
        public int Cost => _cost;
        public virtual void Upgrade()
        {
            for (int i = 0; i < _enableObjects.Length; i++)
            {
                _enableObjects[i].SetActive(true);
            }
            for (int i = 0; i < _disableObjects.Length; i++)
            {
                _disableObjects[i].SetActive(false);
            }

            if (_eventName == string.Empty)
                return;
        }

        public void SetCost(int cost) => _cost = cost;
    }
}