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
        float _width;
        float _height;

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

            _width = _spawnMaxTr.position.x - _spawnMinTr.position.x;
            _height = _spawnMaxTr.position.z - _spawnMinTr.position.z;
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

        private void Update()
        {
            if (!_isResawning)
                return;

            if (_lastRespawnTime + _respawnTime > Time.time)
                return;

            RespawnMole();

        }

        private void RespawnMole()
        {
            _lastRespawnTime = Time.time;

            int xp;
            if (OnGetPlayerXp != null)
                xp = (int)(OnGetPlayerXp.Invoke() * _respawnXpGap) + UnityEngine.Random.Range(1, 100);
            else
                xp = 1;

            Vector3 pos = _target.position + MathUtil.RandomOnCircle(_respawnDistance);

            if (pos.x < _spawnMinTr.position.x)
            {
                pos.x += _width;
            }
            else if (pos.x > _spawnMaxTr.position.x)
            {
                pos.x -= _width;
            }

            if (pos.z < _spawnMinTr.position.z)
            {
                pos.z += _height;
            }
            else if (pos.z > _spawnMaxTr.position.z)
            {
                pos.z -= _height;
            }

            _respawnLine.Dequeue()
            .Respawn(xp)
            .transform.position = pos;

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