using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class Joystick
    {
        [Header("Image")]
        [SerializeField] GameObject _background;
        [SerializeField] GameObject _handle;
        Canvas _canvas;
        RectTransform _backgroundRect;
        RectTransform _handleRect;


        [Space(10f)]
        [Header("Clamp")]
        [SerializeField] float _maxDistanceRatio = 1f;

        Vector2 _direction = Vector2.zero;
        Vector2 _touchPos = Vector2.zero;
        float _maxDistance = 0f;

        public Vector2 Direction => _direction;

        public void SetCanvas(Canvas canvas)
        {
            _canvas = canvas;
        }
        public void Init()
        {
            if (_canvas == null)
            {
                Debug.LogWarning("Canvas For Joystick Is Not Setted!");
                return;
            }

            if (_background != null)
            {
                _backgroundRect = _background.GetComponent<RectTransform>();
            }
            if (_handle != null)
            {
                _handleRect = _handle.GetComponent<RectTransform>();
            }
            _maxDistance = _maxDistanceRatio * (_backgroundRect.sizeDelta.y / 2f) * _canvas.scaleFactor;
        }

        public void Release()
        {
        }
        public void TurnOff()
        {
            _background.SetActive(false);
            _direction = Vector2.zero;
        }
        public void FakeTurnOff()
        {
            _background.SetActive(false);
            _direction = Vector2.zero;
        }

        public void TurnOn()
        {
            _background.SetActive(true);
            _touchPos = Input.mousePosition;

            if (Vector2.Distance(_touchPos, _backgroundRect.position) > _maxDistance)
            {
                Vector2 handleGap = _touchPos - (Vector2)_backgroundRect.position;
                _handleRect.anchoredPosition = handleGap.normalized * _maxDistance / _canvas.scaleFactor;
            }
            else
            {
                _handleRect.position = _touchPos;
            }
            _direction = (_handleRect.position - _backgroundRect.position).normalized;

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _touchPos = eventData.position;
            _background.SetActive(true);
            _background.transform.position = _touchPos;
            _handleRect.anchoredPosition = Vector2.zero;
        }

        public void OnPointerDown(Vector3 pos)
        {
            _touchPos = pos;
            _background.SetActive(true);
            _background.transform.position = _touchPos;
            _handleRect.anchoredPosition = Vector2.zero;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _background.SetActive(false);
            _direction = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _background.SetActive(true);
            _touchPos = eventData.position;
            if (Vector2.Distance(_touchPos, _backgroundRect.position) > _maxDistance)
            {
                Vector2 handleGap = _touchPos - (Vector2)_backgroundRect.position;
                _handleRect.anchoredPosition = handleGap.normalized * _maxDistance / _canvas.scaleFactor;
            }
            else
            {
                _handleRect.position = _touchPos;
            }
            _direction = (_handleRect.position - _backgroundRect.position).normalized;
        }

        internal void SetActive(bool on)
        {
            _background.gameObject.SetActive(false);
        }
    }
}