using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class CustomerSpawner : IInitable, IUpdateable, IBindable
    {
        public event Action<Customer> OnSpawn;

        [Header("Spawn")]
        [SerializeField] MonoBehaviour _coroutineOwner;
        [SerializeField] ObjectPoolHandler _customerPool;

        [SerializeField] float _spawnDuration = 3;
        [SerializeField] int _maxSpawnCount = 3;
        [SerializeField] int _maxFieldCount = 6;

        [Space(10f)]
        [Header("Transforms")]
        [SerializeField] Transform _spawnPos;

        float _lastSpawnTime = 0f;
        int _totalSpawnCount = 0;
        int _currentFieldCount = 0;
        Coroutine _coroutine;
        public void Init()
        {
            _customerPool.Init();
        }

        public void Release()
        {
            _coroutineOwner.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        public void UpdateManualy(float dt)
        {
            if (_currentFieldCount >= _maxFieldCount)
                return;
            if (_lastSpawnTime + _spawnDuration > Time.time)
                return;
            if (_totalSpawnCount >= _maxSpawnCount)
                return;

            MakeCustomer();
        }

        public void AlertCustomerRelease()
        {
            _currentFieldCount--;
        }

        private void MakeCustomer()
        {
            Customer customer = _customerPool.Pool.Get() as Customer;
            customer.transform.position = _spawnPos.position;
            customer.transform.rotation = _spawnPos.rotation;
            _totalSpawnCount++;
            _currentFieldCount++;

            _lastSpawnTime = Time.time;

            OnSpawn?.Invoke(customer);
        }

        public void SetMaxCustomer(int count) => _maxSpawnCount = count;
#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _customerPool.SetPoolParent(mono.transform);
            _customerPool.Bind(mono);
        }
#endif // UNITY_EDITOR
    }
}
