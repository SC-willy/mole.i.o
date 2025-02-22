using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class UnlockPadCanvas
    {
        public event Action OnStackSound;

        [Header("View")]
        [SerializeField] Image _valueSlider;
        [SerializeField] TMP_Text _costText;

        [Space]
        [Header("Sound")]
        [SerializeField] float _soundDuration = 0.05f;

        float _lastSoundTime = 0;
        public void UpdateGauge(float value)
        {
            _valueSlider.fillAmount = value;
        }

        public void UpdateText(int leftMoney) => _costText.text = leftMoney.ToString();

        public void CheckInputSound()
        {
            if (_lastSoundTime + _soundDuration > Time.time)
                return;
            _lastSoundTime = Time.time;
            OnStackSound?.Invoke();
        }

    }
}