using System.Collections;
using System.Collections.Generic;
using Supercent.MoleIO.Management;
using UnityEngine;
using UnityEngine.UI;

namespace Supercent.MoleIO.InGame
{
    public class OptionUI : MonoBehaviour
    {
        [SerializeField] Toggle _sound;
        [SerializeField] Toggle _haptic;

        public void OpenWindow()
        {
            _sound.isOn = PlayerData.IsVolumeOn == 1;
            _haptic.isOn = PlayerData.IsHaptic == 1;
        }
    }
}