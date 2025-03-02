using System;
using System.Collections;
using Supercent.Util;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class IngameMainMediator : IBindable, IStartable, IUpdateable
    {
        [Header("Managers")]

        [CustomColor(0f, 0f, 0f)]
        [SerializeField] FlowEventManager _flowManager = new FlowEventManager();
        [CustomColor(0.2f, 0f, 0.1f)]
        [SerializeField] CameraManager _cameraManager = new CameraManager();
        [CustomColor(0f, 0.1f, 0.2f)]
        [SerializeField] BossFlowManager _bossFlowManager = new BossFlowManager();
        [SerializeField] float _bossFlowDelay = 1f;

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
            _playMediator.SetKillEvent(_mainCanvas.CountKill);
            _player.Attacker.OnSetSize += LevelUpZoom;
            _player.OnDie += OpenFailUI;

            _bossFlowManager.Init();
            _bossFlowManager.OnWin += OpenWinUI;
            _bossFlowManager.OnFail += OpenFailUI;

            _timer.OnEnd += ShowTimerEnd;
        }

        public void UpdateManualy(float dt)
        {
            _bossFlowManager.UpdateManualy(dt);
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
            _leaderBoard.gameObject.SetActive(true);
            _timer.gameObject.SetActive(true);
            _timer.StartTimer();
            _player.StartUpdate();
        }

        private void ShowTimerEnd()
        {
            _player.Release();
            _mainCanvas.SetActiveTimerEndUI();
            _timer.StartCoroutine(CoStartBossFlow());
        }

        private IEnumerator CoStartBossFlow()
        {
            yield return CoroutineUtil.WaitForSeconds(_bossFlowDelay);
            _bossFlowManager.StartFlow(_player.GetPlayerXp());
            _mainCanvas.SetActiveTimerEndUI(false);
        }

        private void OpenWinUI()
        {
            _mainCanvas.OpenWinUI();
        }

        private void OpenFailUI()
        {
            _mainCanvas.OpenFailUI();
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