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
        [Header("Mediators")]
        [SerializeField] MainCanvasMediator _mainCanvas;
        [SerializeField] GamePlayMediator _playMediator;
        [SerializeField] PlayerMediator _player;

        [Space]
        [Header("Others")]
        [SerializeField] IsoCamSizeFitter _isoCam;
        [SerializeField] LeaderBoard _leaderBoard;
        [SerializeField] TimerUI _timer;

        public void StartSetup()
        {
            _flowManager.StartFlow();
            _cameraManager.StartSetup();

            _playMediator.RegistLeaderboard(_leaderBoard);
            _player.Attacker.OnSetSize += LevelUpZoom;

            _timer.OnEnd += ShowTimerEnd;
        }

        public void ShowCamPos(int index)
        {
            _cameraManager.StartShowTr(index);
        }

        private void LevelUpZoom(float playerSize)
        {
            _isoCam.ChangeStackedZoom(_player.Attacker.Level);
        }

        public void StartGame()
        {
            _timer.StartTimer();
            _player.StartUpdate();
        }

        private void ShowTimerEnd()
        {
            _player.Release();
            _mainCanvas.ShowTimerEndUI();
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
            _timer = mono.GetComponentInChildren<TimerUI>(true);
        }
#endif // UNITY_EDITOR
    }
}