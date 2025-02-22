using System;
using System.Collections;
using System.Collections.Generic;
using Supercent.Util;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class MoneyArea : BoolTrigger, IMoneyMaker
    {
        //---------------------------- Events ------------------------------------

        public event Action<int> MakeMoney;

        [Header("Default")]
        [SerializeField] ObjectPoolHandler _pool;

        // Select Variate ------------
        //[SerializeField] TransitionFixedPos _appearMotion;
        [SerializeField] TransitionTween _appearMotion;
        // ---------------------------

        [SerializeField] TransitionTween _getMotion;
        [SerializeField] float _motionGenerateDuration = 0.005f;
        [SerializeField] float _motionGetDuration = 0.1f;
        [SerializeField] int _moneyValuePerItem = 5;
        [SerializeField] bool _isThrown = false;
        [Space(15f)]
        [Header("Transform")]
        [SerializeField] Transform _moneyTransform;

        [SerializeField] int _row;
        [SerializeField] int _colum;
        [SerializeField] int _maxHeight = 20;
        [SerializeField] Vector3 _itemSize = Vector3.one;

        [Space(15f)]
        [Header("Optional")]
        [SerializeField] Transform _moneyStartTransform;
        [SerializeField] ParticleSystem _particle;




        Stack<PooledObject> _moneyObjects = new Stack<PooledObject>();
        Transform GetterTr;
        Vector3 _stackPosition;


        int _layerAmounts = 0;
        int _trueMoneyCount = 0;
        int _stackedMoneyCount = 0;
        int _animCount = 0;


        private void Start()
        {
            _maxHeight = _row * _colum * _maxHeight;
            _layerAmounts = _row * _colum;
            _pool.Init();
        }

        private void Update()
        {
            if (_isEnter && _trueMoneyCount > 0)
            {
                GetMoney();
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            _isEnter = _layer.HasLayer(other.gameObject.layer);
            if (_isEnter)
            {
                _onTriggerEnterEvent?.Invoke();

                if (GetterTr == null)
                    GetterTr = other.transform;
            }


        }

        public void EarnMoney(int value)
        {
            _trueMoneyCount += value;

            if (_maxHeight < _stackedMoneyCount)
            {
                return;
            }

            //Select Variation
            //StartCoroutine(CoGenerateMoney(value));
            GenerateMoney(value);

            if (_particle != null)
                _particle.Play();

            _animCount++;
        }

        public void SetEarnMoneyPos(Vector3 pos)
        {
            _moneyStartTransform.position = pos;
        }

        // private IEnumerator CoGenerateMoney(int value)
        // {
        //     int count = value / _moneyValuePerItem;
        //     for (int i = 0; i < count; i++)
        //     {
        //         yield return CoroutineUtil.WaitForSeconds(_motionGenerateDuration);
        //         var newMoney = _pool.Get();
        //         _stackPosition = _moneyTransform.position;
        //         _stackPosition.x += _stackedMoneyCount % _row * _itemSize.x;
        //         _stackPosition.z += _stackedMoneyCount % _layerAmounts / _row * _itemSize.z;
        //         _stackPosition.y += _stackedMoneyCount / _layerAmounts * _itemSize.y;

        //         Transform newMoneyTr = newMoney.transform;
        //         newMoneyTr.position = _isThrown ? _moneyStartTransform.position : _stackPosition;
        //         _moneyObjects.Push(newMoney);

        //         if (i + 1 < count)
        //         {
        //             _appearMotion.StartTransition(newMoney.transform, newMoney.transform.position, _stackPosition);
        //         }
        //         else
        //         {
        //             _appearMotion.StartTransition(newMoney.transform, newMoney.transform.position, _stackPosition, EnableMoneyGet);
        //         }
        //         _stackedMoneyCount++;
        //     }
        // }

        private void GenerateMoney(int value)
        {
            int count = value / _moneyValuePerItem;
            for (int i = 0; i < count; i++)
            {
                var newMoney = _pool.Get();
                _stackPosition = _moneyTransform.position;
                _stackPosition.x += _stackedMoneyCount % _row * _itemSize.x;
                _stackPosition.z += _stackedMoneyCount % _layerAmounts / _row * _itemSize.z;
                _stackPosition.y += _stackedMoneyCount / _layerAmounts * _itemSize.y;

                Transform newMoneyTr = newMoney.transform;
                newMoneyTr.position = _isThrown ? _moneyStartTransform.position : _stackPosition;
                newMoneyTr.rotation = transform.rotation;
                _moneyObjects.Push(newMoney);

                _appearMotion.StartTransition(newMoney.transform, GetterTr);
                _stackedMoneyCount++;
            }
        }

        private void EnableMoneyGet()
        {
            _animCount--;
        }

        public void GetMoney()
        {
            MakeMoney?.Invoke(_trueMoneyCount);

            StartCoroutine(CoMoveForGetMoney(_moneyObjects.ToArray(), _stackedMoneyCount));
            _stackedMoneyCount = 0;
            _trueMoneyCount = 0;
            _moneyObjects.Clear();

            if (_particle != null)
                _particle.Stop();
        }

        IEnumerator CoMoveForGetMoney(PooledObject[] moneyObjects, int count)
        {
            int moneyCount = count;
            int getAmount = (moneyObjects.Length / 10) + 1;
            for (int i = 0; i < moneyObjects.Length; i++)
            {
                if (i % getAmount == 0)
                    yield return CoroutineUtil.WaitForSeconds(_motionGetDuration);
                int index = i;
                _getMotion.StartTransition(moneyObjects[i].transform, GetterTr, moneyObjects[i].ReturnSelf);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green; // 기즈모 색상 지정

            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _colum; j++)
                {
                    for (int k = 0; k < _maxHeight; k++)
                    {
                        Gizmos.DrawSphere(_moneyTransform.position + new Vector3(_itemSize.x * i, _itemSize.y * k, _itemSize.z * j), 0.1f);
                    }
                }
            }
        }
    }
}