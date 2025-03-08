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
        [SerializeField] EnemyManager _enemyManager;
        [SerializeField] float _playDelay = 1f;
        [SerializeField] float _bossFlowDelay = 1f;

        [Space]
        [Header("Mediators")]
        [SerializeField] MainCanvasMediator _mainCanvas;
        [SerializeField] LobbyCanvasMediator _lobbyCanvas;
        [SerializeField] GamePlayMediator _playMediator;
        [SerializeField] PlayerMediator _player;


        [Space]
        [Header("Others")]
        [SerializeField] IsoCamSizeFitter _isoCam;
        [SerializeField] LeaderBoard _leaderBoard;
        [SerializeField] TimerUI _timer;
        [SerializeField] GameObject _startUI;

        public void StartSetup()
        {
            _flowManager.StartFlow();
            _cameraManager.StartSetup();

            _playMediator.RegistLeaderboard(_leaderBoard);
            _player.Attacker.OnSetSize += LevelUpZoom;

            _bossFlowManager.Init();
            _bossFlowManager.OnWin += OpenWinUI;
            _bossFlowManager.OnFail += OpenFailUI;

            _timer.OnEnd += ShowTimerEnd;

            ScreenInputController.OnPointerDownEvent += StartGameOnTouch;
        }
        private void StartGameOnTouch()
        {
            ScreenInputController.OnPointerDownEvent -= StartGameOnTouch;
            StartGame();
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
            _lobbyCanvas.gameObject.SetActive(false);
            _startUI.SetActive(true);
            _playMediator.StartCoroutine(WaitForStartCount());
        }

        IEnumerator WaitForStartCount()
        {
            yield return CoroutineUtil.WaitForSeconds(_playDelay);
            _timer.StartTimer();
            _player.StartUpdate();
            _enemyManager.ActiveBattle(true);
            _mainCanvas.ActiveIngameUI(true);
        }

        private void ShowTimerEnd()
        {
            _player.Release();
            _mainCanvas.SetActiveTimerEndUI();
            _playMediator.StartCoroutine(CoStartBossFlow());
            _enemyManager.ActiveBattle(false);
        }

        private IEnumerator CoStartBossFlow()
        {
            yield return CoroutineUtil.WaitForSeconds(_bossFlowDelay);
            _bossFlowManager.StartFlow(_player.GetPlayerXp());
            _mainCanvas.ActiveIngameUI(false);
        }

        private void OpenWinUI()
        {
            _mainCanvas.OpenWinUI(_player.GetPlayerXp());
        }

        private void OpenFailUI()
        {
            _mainCanvas.OpenFailUI(_player.GetPlayerXp());
            _timer.OnEnd -= ShowTimerEnd;
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _flowManager.Bind(mono);
            _cameraManager.Bind(mono);
            _isoCam = mono.GetComponentInChildren<IsoCamSizeFitter>(true);
            _mainCanvas = mono.GetComponentInChildren<MainCanvasMediator>(true);
            _lobbyCanvas = mono.GetComponentInChildren<LobbyCanvasMediator>(true);
            _playMediator = mono.GetComponentInChildren<GamePlayMediator>(true);
            _leaderBoard = mono.GetComponentInChildren<LeaderBoard>(true);
            _player = mono.GetComponentInChildren<PlayerMediator>(true);
            _timer = mono.GetComponentInChildren<TimerUI>(true);
            _enemyManager = mono.GetComponentInChildren<EnemyManager>(true);
        }


#endif // UNITY_EDITOR
    }
}