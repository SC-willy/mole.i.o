using System;
using Supercent.PrisonLife.Playable003;
using TMPro;
using UnityEngine;
namespace Supercent.PrisonLife.Playable001
{
    [Serializable]
    public class MoneyUI : IInitable
    {
        const float _SOOUND_DURATION = 0.02f;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] TMP_Text _moneyText;
        [SerializeField] Animation _animation;
        float _lastPlayTime = 0;

        public void Init()
        {
            MoneyManager.OnEarn += PlayEarnEffect;
            MoneyManager.OnValueChanged += UpdateText;
        }

        public void Release()
        {
            MoneyManager.OnEarn -= PlayEarnEffect;
            MoneyManager.OnValueChanged -= UpdateText;
        }

        private void UpdateText(int value)
        {
            if (_moneyText == null)
                return;

            _moneyText.text = value.ToString();

            if (_animation == null)
                return;
            _animation.Play();
        }
        private void PlayEarnEffect()
        {
            if (_audioSource == null)
                return;
            if (Time.time < _lastPlayTime + _SOOUND_DURATION)
                return;

            _lastPlayTime = Time.time;
            _audioSource.Play();
        }
    }
}