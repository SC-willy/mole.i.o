using System;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class EnemyManager : InitManagedBehaviorBase
    {
        public event Func<int> OnGetPlayerXp;
        [SerializeField] EnemyController[] _enemies;
        [SerializeField] Transform _target;
        [SerializeField] Transform _spawnMinTr;
        [SerializeField] Transform _spawnMaxTr;
        [SerializeField] float _respawnTime;
        [SerializeField] float _respawnDistance;
        [SerializeField] float _respawnXpGap = 1.35f;

        float _lastRespawnTime = 0;

        Queue<EnemyController> _respawnLine = new Queue<EnemyController>();
        bool _isResawning;
        protected override void _Init()
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].OnHit += StartHitAction;
                _enemies[i].OnDie += SetRespawnMod;
            }
        }

        private void StartHitAction(EnemyController target)
        {
            target.gameObject.SetActive(false);
        }

        private void SetRespawnMod(EnemyController target)
        {
            _respawnLine.Enqueue(target);

            if (_isResawning)
                return;

            _isResawning = true;
            _lastRespawnTime = Time.time;
        }

        void Update()
        {
            if (!_isResawning)
                return;

            if (_lastRespawnTime + _respawnTime > Time.time)
                return;
            int xp;
            if (OnGetPlayerXp != null)
                xp = (int)(OnGetPlayerXp.Invoke() * _respawnXpGap) + UnityEngine.Random.Range(1, 100);
            else
                xp = 1;

            _lastRespawnTime = Time.time;
            _respawnLine.Dequeue()
            .Respawn(xp)
            .transform.position = _target.position + MathUtil.RandomOnCircle(_respawnDistance);

            if (_respawnLine.Count <= 0)
            {
                _isResawning = false;
            }
        }

        protected override void _Release()
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].OnHit -= StartHitAction;
                _enemies[i].OnDie -= SetRespawnMod;
            }
        }
#if UNITY_EDITOR

        protected override void OnBindSerializedField()
        {
            _enemies = GetComponentsInChildren<EnemyController>();
        }
#endif //UNITY_EDITOR
    }
}