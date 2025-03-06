using System;
using UnityEngine;


namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class CameraManager : IStartable, IBindable
    {
        public static event Action OnCamTrChanged;
        public static event Action<bool> OnCheckCameraWait;
        public static event Action<bool> OnCheckCameraMove;
        event Action _onWaitEnd;

        [SerializeField] Camera _camera;
        [SerializeField] Transform _target;
        [SerializeField] Transform _originTr;
        [SerializeField] Transform _watchAnchor;
        [SerializeField] CameraTransitionData _cameraPosData;
        [SerializeField] Transform[] _watchTransforms;
        [SerializeField] MonoBehaviour[] _disableComponents;
        Vector3 _gap = Vector3.zero;

        [SerializeField] float _customCamMoveTime = 1f;
        [SerializeField] float _customCamWaitTime = 1f;
        [SerializeField] bool _isSmoothCamera = true;
        bool _isCustomMod = false;

        public void SetCustomTimeMode(bool isCustom) => _isCustomMod = isCustom;
        public void ResetCameraPos()
        {
            _camera.transform.position = _originTr.position;
        }

        public void StartSetup()
        {
            if (_camera == null)
                _camera = Camera.main;

            ResetCameraPos();
            _gap = _camera.transform.position - _target.position;
            _originTr.SetParent(_target);

            //if Need Auto Follow
            _camera.transform.SetParent(_originTr);
            OnCamTrChanged?.Invoke();
        }
        private void EnableComponents(bool isEnabled)
        {
            for (int i = 0; i < _disableComponents.Length; i++)
            {
                _disableComponents[i].enabled = isEnabled;
            }
        }
        public void StartShowTr(int index)
        {
            _watchAnchor.transform.SetParent(_watchTransforms[index]);
            OnCamTrChanged?.Invoke();
            _watchAnchor.transform.position = _watchTransforms[index].position + _gap;
            _watchAnchor.transform.rotation = Camera.main.transform.rotation;
            StartShow();
        }
        public void CheackShowTr(int index)
        {
            Vector3 _worldToViewportPoint = _camera.WorldToViewportPoint(_watchTransforms[index].position);

            if (_worldToViewportPoint.x > 0
            && _worldToViewportPoint.x < 1
            && _worldToViewportPoint.y > 0
            && _worldToViewportPoint.y < 1)
                StartShowTr(index);
        }
        public void StartShowPos(int index)
        {
            _watchAnchor.transform.SetParent(null);
            OnCamTrChanged?.Invoke();
            var posData = _cameraPosData.GetTransform(index);
            _watchAnchor.position = posData.position;
            _watchAnchor.rotation = posData.rotation;
            StartShow();
        }

        private void StartShow()
        {
            ScreenInputController.StopJoystickHard();
            EnableComponents(false);

            if (_isCustomMod)
                _cameraPosData.SetCustomMoveMode(_customCamMoveTime);
            else
                _cameraPosData.SetMoveMode();

            _cameraPosData.StartTransition(_camera.transform, _camera.transform, _watchAnchor, StartWait);
            OnCheckCameraMove?.Invoke(true);
        }

        private void StartWait()
        {
            if (_isCustomMod)
                _cameraPosData.SetCustomWaitMode(_customCamWaitTime);
            else
                _cameraPosData.SetWaitMode();

            _cameraPosData.StartTransition(_camera.transform, _watchAnchor, _watchAnchor, EndWait);
            OnCheckCameraWait?.Invoke(true);
        }

        private void EndWait()
        {
            if (_isCustomMod)
                _cameraPosData.SetCustomMoveMode(_customCamMoveTime);
            else
                _cameraPosData.SetMoveMode();

            _cameraPosData.StartTransition(_camera.transform, _watchAnchor, _originTr, EndShow);
            OnCheckCameraWait?.Invoke(false);
            _onWaitEnd?.Invoke();
        }

        private void EndShow()
        {
            ScreenInputController.ActiveJoystick(true);
            OnCheckCameraMove?.Invoke(false);
            _camera.transform.SetParent(_originTr);
            OnCamTrChanged?.Invoke();
            EnableComponents(true);
        }

        public void SetOnWaitEnd(Action action, bool _isRegist)
        {
            if (_isRegist)
                _onWaitEnd += action;
            else
                _onWaitEnd -= action;
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _camera = Camera.main;
            var camData = Resources.FindObjectsOfTypeAll<CameraTransitionData>();

            if (camData == null || camData.Length <= 0)
                return;
            _cameraPosData = camData[0];
        }
#endif // UNITY_EDITOR

    }
}