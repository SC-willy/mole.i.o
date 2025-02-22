using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class IngameMainMediator : IBindable, IStartable
    {

        [CustomColor(0f, 0f, 0f)]
        [SerializeField] FlowEventManager _flowManager = new FlowEventManager();
        [CustomColor(0.2f, 0f, 0.1f)]
        [SerializeField] CameraManager _cameraManager = new CameraManager();

        [Space]
        [Header("Zoom")]
        [SerializeField] IsoCamSizeFitter _isoCam;

        public void StartSetup()
        {
            _flowManager.StartFlow();
            _cameraManager.StartSetup();
        }

        public void ShowCamPos(int index)
        {
            _cameraManager.StartShowTr(index);
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _flowManager.Bind(mono);
            _cameraManager.Bind(mono);
            _isoCam = mono.GetComponentInChildren<IsoCamSizeFitter>(true);
        }
#endif // UNITY_EDITOR
    }
}