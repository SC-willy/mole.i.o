using System;
using System.Collections;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class IsoCamSizeFitter : InitManagedObject
    {
        const int MIN_ORTHO_SIZE = 1;
        const int MAX_ORTHO_SIZE = 8;

        [SerializeField] Camera[] _cameras;
        [SerializeField] float _onWideScreenCameraScale = 1f;
        [SerializeField] AnimationCurve _curve;
        [SerializeField] float _zoomInValue = 1f;
        [SerializeField] float _zoomOutValue = 1f;
        [SerializeField] float _calmpedZoomOutValue = 2;
        [SerializeField] float _cameraOutTime = 0.75f;
        [SerializeField] float _cameraBackTime = 0.75f;
        float _originOrthoSize;
        float _zoomOrthoSize;
        float _currentResolution;
        float _currentOrthosize;
        Coroutine _coroutine;
        [LunaPlaygroundField("Zoom", 0, "Camera")]
        [Range(0, 1.9f)]
        [SerializeField] float _zoomValue = 1f;

        protected override void _Init()
        {
            _zoomValue = 2 - _zoomValue;
            _originOrthoSize = _cameras[0].orthographicSize * _zoomValue;
            _zoomOrthoSize = _originOrthoSize - Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height) * _onWideScreenCameraScale;

            if (_zoomOrthoSize < MIN_ORTHO_SIZE)
            {
                _zoomOrthoSize = MIN_ORTHO_SIZE;
            }
            else if (_zoomOrthoSize > MAX_ORTHO_SIZE)
            {
                _zoomOrthoSize = MAX_ORTHO_SIZE;
            }
            _zoomOrthoSize *= _zoomValue;
            _currentResolution = (float)Screen.width / Screen.height;

            if (Screen.width >= Screen.height)
            {
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize = _zoomOrthoSize;
                }
            }
            else
            {
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize *= _zoomValue;
                }
            }
            _currentOrthosize = _cameras[0].orthographicSize;
        }

        protected override void _Release()
        {
            RemoveJoystickEvent();
        }

        void DefaultZoomOut()
        {
            ZoomOut(_cameraOutTime);
        }

        void DefaultZoomBack()
        {
            ZoomBack(_cameraBackTime);
        }

        public void SetCamera()
        {
            //화면 비율이 변경되면 조정하는 코드
            _currentResolution = Screen.width - Screen.height;
            _zoomOrthoSize = _originOrthoSize - Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height) * _onWideScreenCameraScale * _zoomValue;
            if (Screen.width >= Screen.height)
            {
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize = _zoomOrthoSize;
                }
            }
            else
            {
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize = _originOrthoSize;
                }
            }
            _currentOrthosize = _cameras[0].orthographicSize;
        }

        private void FixedUpdate()
        {
            //런타임 중 기존 비율을 크게 벗어날 시 카메라 줌 변경
            if (Mathf.Abs((float)Screen.width - Screen.height - _currentResolution) < 1f)
                return;
            SetCamera();
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        //      아래 코드들은 가로 / 세로를 런타임 중 변경되더라도 줌인/아웃 처가 정상적으로 보간되도록 구현한 것으로, 필요하지 않으면 사용하지 않아도 된다

        //--------------------------------------------------------------------------------------------------------------------------------

        void AddJoystickEvent()
        {
            ScreenInputController.OnPointerDownEvent += DefaultZoomOut;
            ScreenInputController.OnPointerUpEvent += DefaultZoomBack;
        }

        void RemoveJoystickEvent()
        {
            ScreenInputController.OnPointerDownEvent -= DefaultZoomOut;
            ScreenInputController.OnPointerUpEvent -= DefaultZoomBack;
        }

        public void ZoomIn() => ZoomIn(_cameraOutTime);
        public void ZoomIn(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(StartZoomIn(duration));
        }

        public void ZoomIn(float duration, float zoomValue)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(StartZoomIn(duration, zoomValue));
        }

        public void ZoomOut(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(StartZoomOut(duration));
        }
        public void ZoomOut() => ZoomOut(_cameraOutTime);

        public void ZoomOutInClamp(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            RemoveJoystickEvent();

            _coroutine = StartCoroutine(StartZoomOut(duration, _calmpedZoomOutValue, AddJoystickEvent));
        }

        public void ZoomBack(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(BackOriginZoom(duration));
        }
        public void ZoomBack() => ZoomBack(_cameraBackTime);

        public void ZoomInAndReturn(float duration, float backDuration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(StartZoomIn(duration, () => ZoomBack(backDuration)));
        }

        IEnumerator StartZoomIn(float duration, Action onEnd = null)
        {
            float _timer = 0;
            float lerpValue = 0f;

            float orthoSize;
            float startZoom = _cameras[0].orthographicSize;
            while (lerpValue < 1f)
            {
                _timer += Time.deltaTime;
                lerpValue = Mathf.Min(_timer / duration, 1f);
                orthoSize = Mathf.Lerp(startZoom, _currentOrthosize - _zoomInValue, _curve.Evaluate(lerpValue));
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize = orthoSize;
                }

                yield return null;
            }
            _coroutine = null;

            if (onEnd != null)
            {
                onEnd.Invoke();
            }
        }



        IEnumerator StartZoomIn(float duration, float zoomValue, Action onEnd = null)
        {
            float _timer = 0;
            float lerpValue = 0f;

            float orthoSize;
            float startZoom = _cameras[0].orthographicSize;
            while (lerpValue < 1f)
            {
                _timer += Time.deltaTime;
                lerpValue = Mathf.Min(_timer / duration, 1f);
                orthoSize = Mathf.Lerp(startZoom, _currentOrthosize - zoomValue, _curve.Evaluate(lerpValue));
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize = orthoSize;
                }

                yield return null;
            }
            _coroutine = null;

            if (onEnd != null)
            {
                onEnd.Invoke();
            }
        }

        IEnumerator BackOriginZoom(float duration, Action onEnd = null)
        {
            float _timer = 0;
            float lerpValue = 0f;

            float orthoSize;
            float startZoom = _cameras[0].orthographicSize;
            while (lerpValue < 1f)
            {
                _timer += Time.deltaTime;
                lerpValue = Mathf.Min(_timer / duration, 1f);
                orthoSize = Mathf.Lerp(startZoom, _currentOrthosize, _curve.Evaluate(lerpValue));
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize = orthoSize;
                }

                yield return null;
            }
            _coroutine = null;

            if (onEnd != null)
            {
                onEnd.Invoke();
            }
        }


        IEnumerator StartZoomOut(float duration, Action onEnd = null)
        {
            float _timer = 0;
            float lerpValue = 0f;

            float orthoSize;
            float startZoom = _cameras[0].orthographicSize;
            while (lerpValue < 1f)
            {
                _timer += Time.deltaTime;
                lerpValue = Mathf.Min(_timer / duration, 1f);
                orthoSize = Mathf.Lerp(startZoom, _currentOrthosize + _zoomOutValue, _curve.Evaluate(lerpValue));
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize = orthoSize;
                }

                yield return null;
            }
            _coroutine = null;

            if (onEnd != null)
            {
                onEnd.Invoke();
            }
        }

        IEnumerator StartZoomOut(float duration, float zoomValue, Action onEnd = null)
        {
            float _timer = 0;
            float lerpValue = 0f;

            float orthoSize;
            float startZoom = _cameras[0].orthographicSize;
            while (lerpValue < 1f)
            {
                _timer += Time.deltaTime;
                lerpValue = Mathf.Min(_timer / duration, 1f);
                orthoSize = Mathf.Lerp(startZoom, _currentOrthosize + zoomValue, _curve.Evaluate(lerpValue));
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].orthographicSize = orthoSize;
                }

                yield return null;
            }
            _coroutine = null;

            if (onEnd != null)
            {
                onEnd.Invoke();
            }
        }


    }
}

