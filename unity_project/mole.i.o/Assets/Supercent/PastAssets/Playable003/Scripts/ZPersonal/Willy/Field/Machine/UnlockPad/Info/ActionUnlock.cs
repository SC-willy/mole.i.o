using System;
using UnityEngine;
using UnityEngine.Events;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class ActionUnlock : DefaultUnlock
    {
        [SerializeField] UnityEvent _event;
        public override void Upgrade()
        {
            _event.Invoke();
            base.Upgrade();
        }
    }
}