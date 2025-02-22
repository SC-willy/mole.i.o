using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class MainMediator : IBindable, IStartable
    {
        const int ZOOM_FLOW = 0;
        const int WAIT_UPGRADE_FLOW = 10;
        const int SHOW_MINE_FLOW = 11;
        const int CAM_POS_MINE_NODE = 2;
        const int CAM_POS_MINE = 3;
        const int CAM_POS_JAIL = 4;

        [CustomColor(0f, 0f, 0f)]
        [SerializeField] FlowEventManager _flowManager = new FlowEventManager();
        [CustomColor(0.2f, 0f, 0.1f)]
        [SerializeField] CameraManager _cameraManager = new CameraManager();

        [SerializeField] GamePlayMediator _gamePlayMediator;
        [SerializeField] MainCanvasMediator _canvas;
        [SerializeField] GameObject _mineAreaUnlockNode;
        [SerializeField] GameObject _endCard;

        [Space]
        [Header("Zoom")]
        [SerializeField] IsoCamSizeFitter _isoCam;

        public void StartSetup()
        {
            _flowManager.StartFlow();
            _cameraManager.StartSetup();

            _gamePlayMediator.OnInteractElevator += _canvas.SetActiveElevatorUI;
        }

        public void ShowCamPos(int index)
        {
            _cameraManager.StartShowTr(index);

            if (index != ZOOM_FLOW)
                return;
            _isoCam.ZoomOut();
            CameraManager.OnCheckCameraWait += ZoomBack;
        }

        private void ZoomBack(bool isWait)
        {
            if (isWait)
                return;
            CameraManager.OnCheckCameraWait -= ZoomBack;
            _isoCam.ZoomBack();
        }

        public void StartFlowWaitUpgrade()
        {
            _flowManager.StartFlow(WAIT_UPGRADE_FLOW);
        }

        public void StartMineFlow()
        {
            _flowManager.StartFlow(SHOW_MINE_FLOW);
            OpenMineArea();
        }
        public void StopAllFlow()
        {
            _flowManager.StopAllFlow();
            _gamePlayMediator.StopAllGuide();
        }

        public void OpenMineArea()
        {
            _mineAreaUnlockNode.gameObject.SetActive(true);
            _cameraManager.StartShowTr(CAM_POS_MINE_NODE);
        }

        public void ShowFinalCam()
        {
            _cameraManager.SetCustomTimeMode(true);
            _cameraManager.StartShowTr(CAM_POS_MINE);
            CameraManager.OnCheckCameraWait += SecondFinalCam;
            _isoCam.ZoomOut();
        }

        private void SecondFinalCam(bool _isWait)
        {
            if (_isWait)
                return;
            CameraManager.OnCheckCameraWait -= SecondFinalCam;
            _cameraManager.SetCustomTimeMode(false);
            _cameraManager.StartShowTr(CAM_POS_JAIL);
            _gamePlayMediator.ShowWork();
            _isoCam.ZoomBack();
            CameraManager.OnCheckCameraMove += ActiveEndCardTrigger;
        }
        private void ActiveEndCardTrigger(bool _isMove)
        {
            if (_isMove)
                return;

            CameraManager.OnCheckCameraMove -= ActiveEndCardTrigger;

            ScreenInputController.OnPointerDownEvent += ShowEndCard;
        }
        private void ShowEndCard()
        {
            ScreenInputController.OnPointerDownEvent -= ShowEndCard;
            _endCard.gameObject.SetActive(true);
            ScreenInputController.StopJoystickHard();
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _flowManager.Bind(mono);
            _cameraManager.Bind(mono);
            _gamePlayMediator = mono.GetComponentInChildren<GamePlayMediator>(true);
            _canvas = mono.GetComponentInChildren<MainCanvasMediator>(true);
            _isoCam = mono.GetComponentInChildren<IsoCamSizeFitter>(true);
        }
#endif // UNITY_EDITOR
    }
}