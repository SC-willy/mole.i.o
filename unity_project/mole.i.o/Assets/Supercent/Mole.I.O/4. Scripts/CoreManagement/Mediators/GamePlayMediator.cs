using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class GamePlayMediator : InitManagedBehaviorBase
    {
        [CustomColor(0.2f, 0f, 0f)]
        [SerializeField] HexGrid _map;
        [SerializeField] HexHammer[] _hammers;
        [SerializeField] PlayerMediator _player;
        [SerializeField] EnemyManager _enemyManager;
        protected override void _Init()
        {
            _map.StartSetup();
            for (int i = 0; i < _hammers.Length; i++)
            {
                _hammers[i].SetMapInfo(_map);
            }

            _enemyManager.OnGetPlayerXp += _player.GetPlayerXp;
        }

        public void RegistLeaderboard(LeaderBoard leaderBoard)
        {
            leaderBoard.RegistUnit(_player.Attacker);
            var BattleControllers = _enemyManager.BattleControllers;
            for (int i = 0; i < BattleControllers.Length; i++)
            {
                leaderBoard.RegistUnit(BattleControllers[i]);
            }

            leaderBoard.UpdateScores();
        }

        void Update()
        {
            _map.UpdateManualy(Time.deltaTime);
        }

        public void SetKillEvent(Action<int> action)
        {
            _enemyManager.OnKill += action;
        }

        protected override void _Release()
        {
        }

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            _hammers = GetComponentsInChildren<HexHammer>();
            _player = GetComponentInChildren<PlayerMediator>();
            _enemyManager = GetComponentInChildren<EnemyManager>();

            _map.Bind(this);
        }

#endif // UNITY_EDITOR
    }
}