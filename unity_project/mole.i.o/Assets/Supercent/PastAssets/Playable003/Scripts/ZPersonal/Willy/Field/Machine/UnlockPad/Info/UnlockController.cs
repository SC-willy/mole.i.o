using System;
using System.Collections.Generic;
using Supercent.MoleIO.Playable03;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class UnlockController : IStartable, IBindable
    {
        private enum UpgradeCode
        {
            Escalator = 0,
            Room2 = 2,
            Room3 = 3,
            WorkPlace = 4,
        }

        [SerializeField] UnlockPad[] _upgradeAreas;
        [SerializeField] DefaultUnlock[] _defaultUpgrade;
        [SerializeField] ActionUnlock[] _actionUpgrade;
        Dictionary<int, DefaultUnlock> _upgradeInfo = new Dictionary<int, DefaultUnlock>();

        [Space]
        [Header("Sound")]
        //[SerializeField] AudioSource _inputAudio;
        [SerializeField] PitchUpAudio _inputAudio;
        [SerializeField] AudioSource _upgradeAudio;

        [Space]
        [SerializeField] int _escalatorCost = 10;
        [SerializeField] int _room2Cost = 30;
        [SerializeField] int _room3Cost = 50;
        [SerializeField] int _workPlaceCost = 50;

        public void StartSetup()
        {
            for (int i = 0; i < _defaultUpgrade.Length; i++)
            {
                _upgradeInfo.Add(_defaultUpgrade[i].Code, _defaultUpgrade[i]);
            }
            for (int i = 0; i < _actionUpgrade.Length; i++)
            {
                _upgradeInfo.Add(_actionUpgrade[i].Code, _actionUpgrade[i]);
            }

            for (int i = 0; i < _upgradeAreas.Length; i++)
            {
                UnlockPad area = _upgradeAreas[i];
                area.OnUpgradeFunction += Upgrade;

                if (CheckDynamicCost(area))
                    continue;

                if (!_upgradeInfo.ContainsKey(area.UpgradeCode))
                    continue;

                area.SetCost(_upgradeInfo[area.UpgradeCode].Cost);
            }

            if (_inputAudio != null)
            {
                for (int i = 0; i < _upgradeAreas.Length; i++)
                {
                    _upgradeAreas[i].OnStackSound += _inputAudio.Play;
                }
            }
        }

        private bool CheckDynamicCost(UnlockPad area)
        {
            switch ((UpgradeCode)area.UpgradeCode)
            {
                case UpgradeCode.Escalator:
                    area.SetCost(_escalatorCost);
                    return true;
                case UpgradeCode.Room2:
                    area.SetCost(_room2Cost);
                    return true;
                case UpgradeCode.Room3:
                    area.SetCost(_room3Cost);
                    return true;
                case UpgradeCode.WorkPlace:
                    area.SetCost(_workPlaceCost);
                    return true;
            }

            return false;
        }

        private void Upgrade(int code)
        {
            if (_upgradeInfo.ContainsKey(code))
                _upgradeInfo[code].Upgrade();

            if (_upgradeAudio != null)
                _upgradeAudio.Play();
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _upgradeAreas = mono.GetComponentsInChildren<UnlockPad>(true);
        }
#endif //UNITY_EDITOR
    }
}