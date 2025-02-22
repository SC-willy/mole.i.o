using System.Collections.Generic;
using UnityEngine;
using Supercent.Util;
using TMPro;
using System;

namespace Supercent.MoleIO.InGame
{
    public class MachineBase : InitManagedObject
    {
        public event Action OnStart;
        public Stacker Ouput => _outputStack.Stacker;

        [SerializeField] protected MonoStacker _outputStack;
        [SerializeField] protected ObjectPoolHandler _pool;
        [SerializeField] protected Transform _spawnTr;
        [SerializeField] protected float _spawnDuration = 1f;
        public void SetSpawnDuration(float time) => _spawnDuration = time;
        protected float _lastSpawnTime = 0f;

        protected override void _Init()
        {
            OnStart?.Invoke();
        }

        protected override void _Release()
        {
            OnStart = null;
        }
        protected virtual void OnEnable()
        {
            _lastSpawnTime = Time.time;
        }

        protected virtual void Update()
        {
            if (_lastSpawnTime + _spawnDuration >= Time.time)
                return;
            SpawnItem();
        }


        public virtual void SpawnItem()
        {
            _lastSpawnTime = Time.time;
            if (!Ouput.IsCanGet)
                return;

            StackableItem itemObject = _pool.Get() as StackableItem;
            itemObject.transform.position = _spawnTr.position;

            Ouput.TryGetItem(itemObject);
        }

        public void Upgrade(float spawnCoolTime = -1, Transform spawnTransform = null, Transform itemHoldTransform = null, ObjectPoolHandler pool = null)
        {
            if (spawnCoolTime >= 0f)
                _spawnDuration = spawnCoolTime;

            if (pool != null)
                _pool = pool;

            if (spawnTransform != null)
                _spawnTr = spawnTransform;

            Ouput.ChangeStackPos(itemHoldTransform.position);
        }

        public void LinkInputInventory(MonoStacker output)
        {
            _outputStack = output;
        }
    }
}