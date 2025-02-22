using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Supercent.PrisonLife.Playable003
{
    [CreateAssetMenu(fileName = "CameraManagerPosData", menuName = "Transition/CameraManagerPosData")]
    public class CameraTransitionData : TransitionGenericBase<TransTokenStartTr>
    {

        [System.Serializable]
        public struct CameraTransform
        {
#if UNITY_EDITOR
            public string _tag;
#endif // UNITY_EDITOR
            public Vector3 position;
            public Quaternion rotation;
        }


        [Header("Motion")]
        [SerializeField] protected AnimationCurve _curve;
        [LunaPlaygroundField("Camera Move Time", 0, "Camera")]
        [SerializeField] float _cameraMoveTime = 1f;
        [LunaPlaygroundField("Camera Move Wait Time", 0, "Camera")]
        [SerializeField] float _cameraMoveWaitTime = 1f;


        [Space]
        [Header("Position & Rotation")]
        [SerializeField] private List<CameraTransform> cameraTransforms = new List<CameraTransform>();

        public void SetMoveMode()
        {
            _duration = _cameraMoveTime;
            _inversedDuration = 1 / _cameraMoveTime;
        }

        public void SetWaitMode()
        {
            _duration = _cameraMoveWaitTime;
            _inversedDuration = 1 / _cameraMoveWaitTime;
        }

        public void SetCustomMoveMode(float moveTime)
        {
            _duration = moveTime;
            _inversedDuration = 1 / moveTime;
        }

        public void SetCustomWaitMode(float waitTime)
        {
            _duration = waitTime;
            _inversedDuration = 1 / waitTime;
        }

        public override void _UpdateTransition(TransTokenStartTr token)
        {
            token.Target.position = Vector3.Lerp(token.StartPos, token.End.position, _curve.Evaluate(token.Time));
            token.Target.rotation = Quaternion.Lerp(token.StartRot, token.End.rotation, _curve.Evaluate(token.Time));
        }

        public void AddTransform(Vector3 position, Quaternion rotation)
        {
            cameraTransforms.Add(new CameraTransform { position = position, rotation = rotation });
        }

        public List<CameraTransform> GetTransforms() => cameraTransforms;
        public CameraTransform GetTransform(int index) => cameraTransforms[index];

        public void ClearTransforms()
        {
            cameraTransforms.Clear();
        }

        public void SaveCurrentCameraTransform()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("메인 카메라가 없습니다!");
                return;
            }

            cameraTransforms.Add(new CameraTransform
            {
                position = mainCamera.transform.position,
                rotation = mainCamera.transform.rotation
            });
        }
    }
#if UNITY_EDITOR

    [CustomEditor(typeof(CameraTransitionData))]
    public class CameraTransformListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CameraTransitionData script = (CameraTransitionData)target;

            if (GUILayout.Button("📸 현재 카메라 위치 저장"))
            {
                script.SaveCurrentCameraTransform();
            }
        }
    }
#endif
}