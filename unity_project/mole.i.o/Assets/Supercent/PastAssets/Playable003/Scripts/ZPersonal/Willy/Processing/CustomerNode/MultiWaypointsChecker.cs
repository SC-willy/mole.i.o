using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class MultiWaypointsChecker : InitManagedObject
    {
        protected Action _onAdd;
        public event Action OnAdd { add => _onAdd += value; remove => _onAdd -= value; }
        [SerializeField] protected MultiWaypoint[] _waypoints;
        protected bool[] _enabled;
        protected override void _Init()
        {
            _enabled = new bool[_waypoints.Length];
            for (int i = 0; i < _enabled.Length; i++)
            {
                _enabled[i] = true;
            }
        }

        protected override void _Release()
        {

        }
        public virtual int GetLeftIndexForUse()
        {
            for (int i = 0; i < _enabled.Length; i++)
            {
                if (_enabled[i])
                {
                    _enabled[i] = false;
                    _onAdd?.Invoke();
                    return i;
                }

            }
            return -1;
        }

        public virtual int GetLeftIndex()
        {
            for (int i = 0; i < _enabled.Length; i++)
            {
                if (_enabled[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public Transform[] GetRouteTr(int i)
        {
            return _waypoints[i].Points;
        }

        public Vector3[] GetRoute(int i)
        {
            Vector3[] newVector = new Vector3[_waypoints[i].Points.Length];
            for (int j = 0; j < newVector.Length; j++)
            {
                newVector[j] = _waypoints[i].Points[j].position;
            }
            return newVector;
        }

        public void CompleteUse(int i)
        {
            _enabled[i] = true;
        }

        public void SetOnCustomerAdd(Action action, bool _isRegist)
        {
            if (_isRegist)
                OnAdd += action;
            else
                OnAdd -= action;
        }

    }

    [Serializable]
    public class MultiWaypoint
    {
        [SerializeField] Transform[] _points;
        public Transform[] Points { get { return _points; } }
    }
}