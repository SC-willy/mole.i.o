using System;
using UnityEngine;
using UnityEngine.UI;

namespace Supercent.WaterParkBoys.Playable002
{
    public class ProcessingArea : MonoBehaviour
    {
        const string ON_ANIMATION = "CalculateOn";
        const string OFF_ANIMATION = "CalculateOff";

        public Action OnFunction;

        [SerializeField] SpriteRenderer _footHold;
        [SerializeField] GameObject _progressUI;
        [SerializeField] Image _gauge;

        [SerializeField] Collider _collider;
        [SerializeField] Animation _animation;
        [SerializeField] Animation _iconAnimation;        
        
        public float GaugeFillTime = 1f;
        
        bool _canUse = false;
        bool _isEnter = false;
        bool _isAutoMod = false;
        bool _isCalculating = false;
        float _progress = 0f;


        private void OnTriggerEnter(Collider other)
        {
            _isEnter = true;
        }

        private void OnTriggerExit(Collider other)
        {
            _isEnter = false;
            _iconAnimation.Stop();
        }
        private void Update()
        {
            if (!_canUse)
                return;

            if (_isEnter || _isAutoMod)
            {
                if (!_isCalculating)
                {
                    _isCalculating = true;
                    _progressUI.SetActive(true);
                    _iconAnimation.Play();
                }
                _progress = Mathf.Min(_progress + (Time.deltaTime / GaugeFillTime), 1f);
                _gauge.fillAmount = _progress;

                if (_progress < 1f)
                    return;

                _canUse = false;
                _progress = 0f;
                _gauge.fillAmount = _progress;
                if (OnFunction == null)
                    return;
                OnFunction.Invoke();
            }
            else
            {
                if (!_isCalculating)
                    return;

                _progress = Mathf.Max(_progress - (Time.deltaTime / GaugeFillTime * 2), 0f);
                _gauge.fillAmount = _progress;

                if (_progress > 0f)
                    return;
                _isCalculating = false;
            }
        }

        public void ReUsableArea()
        {
            _canUse = true;
            _isCalculating = false;
            _progress = 0;

            _animation.Play(ON_ANIMATION);
        }

        public void Automation(bool on = true)
        {
            _isAutoMod = on;
            _collider.enabled = !on;

            if (_footHold == null)
                return;
            _footHold.enabled = !on;
        }

        public void AnimateOff()
        {
            _animation.Play(OFF_ANIMATION);
        }
    }
}