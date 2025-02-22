using UnityEngine;
using Supercent.Util;
using System;


#if UNITY_EDITOR
using Unity.VisualScripting;
#endif

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class ObjectPoolHandler : IInitable, IBindable
    {
        public PooledObject Prefab => _prefab;
        public ObjectPool<PooledObject> Pool => _pool;


        [SerializeField] protected PooledObject _prefab = null;
        [SerializeField] protected Transform _poolParent = null;
        [SerializeField] protected int _defaultCapacity = 16;
        [SerializeField] protected int _prebakeCount = 0;

        [Space]
        [Header("BakedInfo")]
        [SerializeField] protected PooledObject[] _bakedObjects = null;
        protected ObjectPool<PooledObject> _pool;
        protected PooledObject _madePoolObjectCash;
        public void Init()
        {
            _pool = new ObjectPool<PooledObject>(_prefab, MakePooledObject, _defaultCapacity);
            for (int i = 0; i < _bakedObjects.Length; i++)
            {
                if (_bakedObjects[i] != null)
                {
                    _madePoolObjectCash.Parent = _poolParent;
                    _bakedObjects[i].OwnPool = _pool;
                }
            }
        }
        public void Init(Transform parent)
        {
            _poolParent = parent;
            Init();
        }

        private PooledObject MakePooledObject(PooledObject prefab)
        {
            _madePoolObjectCash = UnityEngine.Object.Instantiate(prefab, _poolParent);
            _madePoolObjectCash.Parent = _poolParent;
            _madePoolObjectCash.OwnPool = _pool;
            return _madePoolObjectCash;
        }

        public void Release() => _pool.Clear();
        public PooledObject Get() => _pool.Get();
        public void Return(PooledObject poolObject) => _pool.Return(poolObject);
        public void SetPoolParent(Transform parent) => _poolParent = parent;

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            int deleteCount = Mathf.Max(_bakedObjects.Length - _prebakeCount, 0);
            int createCount = Mathf.Max(_prebakeCount - _bakedObjects.Length, 0);

            int indexStart = Mathf.Max(_prebakeCount - 1, 0);
            int leftCount = Mathf.Min(_bakedObjects.Length, _prebakeCount);

            for (int i = 0; i < deleteCount; i++)
            {
                if (_bakedObjects[i + indexStart] != null)
                    UnityEngine.Object.DestroyImmediate(_bakedObjects[i + indexStart].gameObject);
            }
            var _newArray = new PooledObject[_prebakeCount];

            for (int i = 0; i < leftCount; i++)
            {
                _newArray[i] = _bakedObjects[i];
                _bakedObjects[i].transform.SetParent(_poolParent);
            }

            if (createCount <= 0)
            {
                _bakedObjects = _newArray;
                return;
            }

            for (int i = leftCount; i < createCount; i++)
            {
                PooledObject item = UnityEditor.PrefabUtility.InstantiatePrefab(_prefab).GetComponentInChildren<PooledObject>();
                item.transform.SetParent(_poolParent);
                _newArray[i] = item;
            }
            _bakedObjects = null;
            _bakedObjects = _newArray;
        }
#endif // UNITY_EDITOR
    }
}