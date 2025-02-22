using UnityEngine;
using UnityEngine.UI;
using System;

namespace Supercent.MoleIO.InGame
{
    public class BubbleCanvas : MonoBehaviour
    {
        [SerializeField] private TransitionTween _takeAnim;
        [SerializeField] private GameObject _objCanvas = null;
        [SerializeField] private Image _imgSprite = null;
        [SerializeField] private Image _imgFill = null;
        [SerializeField] private GameObject _exitExpIcon = null;
        [SerializeField] private float _fillSpeed = 3;

        bool _isFill = false;
        Action _callBack;

        public void IsActivityExpIcon(bool isActive)
        {
            _exitExpIcon.SetActive(isActive);
        }

        protected void Start()
        {
            _imgSprite.rectTransform.sizeDelta = new Vector2(1, 1);
            _imgFill.rectTransform.sizeDelta = new Vector2(1, 1);
        }
        public void Show()
        {
            _imgFill.fillAmount = 0f;
            _objCanvas.SetActive(true);
        }
        public void Hide()
        {
            StopFillGauge();
            gameObject.SetActive(false);
        }
        public void StartFillGague(Action callback = null)
        {
            _imgFill.fillAmount = 0;
            _imgFill.gameObject.SetActive(true);
            _isFill = true;
            _callBack = callback;

            if (_takeAnim != null)
                _takeAnim.StartTransition(_objCanvas.transform, _objCanvas.transform);
        }
        private void Update()
        {
            transform.rotation = Camera.main.transform.rotation;

            if (!_isFill)
                return;

            _imgFill.fillAmount = Mathf.Clamp01(_imgFill.fillAmount + _fillSpeed * Time.deltaTime);

            if (_imgFill.fillAmount >= 1f)
            {
                _isFill = false;
                _callBack?.Invoke();
                _callBack = null;
            }
        }

        public void StopFillGauge()
        {
            _isFill = false;
            _imgFill.fillAmount = 0;
        }
    }
}