using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    /// <summary>
    /// 조이스틱의 방향에 따라, 카메라 각도를 더 비춰주는 기능
    /// 카메라에 부착해서 사용
    /// </summary>
    public class SmoothCameraByJoyStick : MonoBehaviour
    {
        private Vector3 _cameraDefaultAngle;


        [Header("Option")]
        [Tooltip("회전 가능한 최대 각도")]
        [SerializeField] private Vector3 _tiltAngles;

        [Tooltip("회전 속도 (최대 각도가 크면, 원하는 최대 회전 각도에 도달하는 데 시간이 더 오래 걸림)")]
        [SerializeField][Range(1, 10)] private float _rotSpeed;

        [Tooltip("회전 민감도 (조이스틱 입력에 따른 회전 민감도.\n해당 값이 높으면 조이스틱 입력에 대해 카메라가 더 크게 회전/낮으면 회전이 작아짐)")]
        [SerializeField][Range(0, 50)] private float _tiltSensitivity;

        Quaternion _targetRotation;
        bool _isControlled = false;

        void Start()
        {
            _cameraDefaultAngle = transform.localRotation.eulerAngles;
            CameraManager.OnCamTrChanged += ResetAngle;
        }

        void OnDestroy()
        {
            CameraManager.OnCamTrChanged -= ResetAngle;
        }

        public void ResetAngle() => _cameraDefaultAngle = transform.localRotation.eulerAngles;
        private void StartTilt() => _isControlled = true;
        private void EndTilt() => _isControlled = false;

        void FixedUpdate()
        {
            if (!_isControlled)
            {
                _targetRotation = Quaternion.Euler(_cameraDefaultAngle);
                ScreenInputController.OnPointerDownEvent += StartTilt;
                ScreenInputController.OnPointerUpEvent += EndTilt;
            }
            else
            {
                Vector3 rot = CalculateTiltAngle();

                var targetRot = new Vector3
                (
                    Mathf.Clamp(rot.x, -_tiltAngles.x, _tiltAngles.x),
                    Mathf.Clamp(rot.y, -_tiltAngles.y, _tiltAngles.y),
                    Mathf.Clamp(rot.z, -_tiltAngles.z, _tiltAngles.z)
                ) + _cameraDefaultAngle;

                _targetRotation = Quaternion.Euler(targetRot);
            }

            transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, Time.deltaTime * _rotSpeed);
        }
        private Vector3 CalculateTiltAngle()
        {
            var angle = -(ScreenInputController.Direction.y * Vector3.right) + ScreenInputController.Direction.x * Vector3.up;
            return angle * _tiltSensitivity;
        }
    }
}