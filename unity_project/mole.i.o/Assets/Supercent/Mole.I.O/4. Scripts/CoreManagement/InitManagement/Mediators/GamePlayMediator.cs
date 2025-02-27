using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class GamePlayMediator : InitManagedBehaviorBase
    {
        [CustomColor(0.2f, 0f, 0f)]
        [SerializeField] HexGrid _map;
        [SerializeField] HexHammer[] _hammers;
        protected override void _Init()
        {
            _map.StartSetup();
            for (int i = 0; i < _hammers.Length; i++)
            {
                _hammers[i].SetMapInfo(_map);
            }
        }

        void Update()
        {
            _map.UpdateManualy(Time.deltaTime);
        }

        protected override void _Release()
        {
        }

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            _hammers = GetComponentsInChildren<HexHammer>();
            _map.Bind(this);
        }

#endif // UNITY_EDITOR
    }
}