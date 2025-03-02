using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class IngameMainMediator : IBindable, IStartable
    {
        [Header("Managers")]

        [CustomColor(0f, 0f, 0f)]
        [SerializeField] FlowEventManager _flowManager = new FlowEventManager();
        [CustomColor(0.2f, 0f, 0.1f)]
        [SerializeField] CameraManager _cameraManager = new CameraManager();

        [Space]
        [Header("Others")]
        [SerializeField] MainCanvasMediator _mainCanvas;
        [SerializeField] GamePlayMediator _playMediator;
        [SerializeField] PlayerMediator _player;
        [SerializeField] LeaderBoard _leaderBoard;
        [SerializeField] IsoCamSizeFitter _isoCam;

        public void StartSetup()
        {
            _flowManager.StartFlow();
            _cameraManager.StartSetup();

            _playMediator.RegistLeaderboard(_leaderBoard);
            _player.Attacker.OnSetSize += LevelUpZoom;
        }

        public void ShowCamPos(int index)
        {
            _cameraManager.StartShowTr(index);
        }

        private void LevelUpZoom(float playerSize)
        {
            _isoCam.ChangeStackedZoom(_player.Attacker.Level);
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _flowManager.Bind(mono);
            _cameraManager.Bind(mono);
            _isoCam = mono.GetComponentInChildren<IsoCamSizeFitter>(true);
            _mainCanvas = mono.GetComponentInChildren<MainCanvasMediator>(true);
            _playMediator = mono.GetComponentInChildren<GamePlayMediator>(true);
            _leaderBoard = mono.GetComponentInChildren<LeaderBoard>(true);
            _player = mono.GetComponentInChildren<PlayerMediator>(true);
        }
#endif // UNITY_EDITOR
    }
}