using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public abstract class InitManagedObject : InitCheckObject, IComparable<InitManagedObject>
    {
        [Header("Init Sort Priority")]
        [SerializeField] private int _callPriority = 0;
        public int CallPriority => _callPriority;

        public int CompareTo(InitManagedObject other)
        {
            return other.CallPriority.CompareTo(CallPriority);
        }
    }

}
