using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif // UNITY_EDITOR

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class MoneyManager : IInitable, IBindable
    {
        private static MoneyData _data = new MoneyData();
        public static event Action OnEarn { add { _data.OnEarn += value; } remove { _data.OnEarn -= value; } }
        public static event Action<int> OnValueChanged { add { _data.OnValueChanged += value; } remove { _data.OnValueChanged -= value; } }

        public static void Earn(int value) => _data.Earn(value);
        public static void Use(int value) => _data.Use(value);
        public static void ResetMoney(int value) => _data.Reset(value);
        public static int GetMoneyValue() => _data.Value;
        public static int Money => _data.Value;

        [SerializeField] MonoBehaviour[] _reader;
        [SerializeField] MonoBehaviour[] _maker;
        [SerializeField] MonoBehaviour[] _user;
        [SerializeField] int _defaultMoney;

        public void Init()
        {
            for (int i = 0; i < _reader.Length; i++)
            {
                Regist(_reader[i] as IMoneyValueReader);
            }
            for (int i = 0; i < _maker.Length; i++)
            {
                Regist(_maker[i] as IMoneyMaker);
            }
            for (int i = 0; i < _user.Length; i++)
            {
                Regist(_user[i] as IMoneyUser);
            }

            _data.Reset(_defaultMoney);
        }

        public void Release()
        {
            for (int i = 0; i < _reader.Length; i++)
            {
                UnRegist(_reader[i] as IMoneyValueReader);
            }
            for (int i = 0; i < _maker.Length; i++)
            {
                UnRegist(_maker[i] as IMoneyMaker);
            }
            for (int i = 0; i < _user.Length; i++)
            {
                UnRegist(_user[i] as IMoneyUser);
            }
        }

        public void Regist(IMoneyValueReader moneyValueGetters)
        {
            moneyValueGetters.GetMoneyValue += _data.GetValue;
        }

        public void Regist(IMoneyMaker moneyMaker)
        {
            moneyMaker.MakeMoney += _data.Earn;
        }

        public void Regist(IMoneyUser moneyUser)
        {
            moneyUser.UseMoney += _data.Use;
        }

        public void UnRegist(IMoneyValueReader moneyValueGetters)
        {
            moneyValueGetters.GetMoneyValue -= _data.GetValue;
        }

        public void UnRegist(IMoneyMaker moneyMaker)
        {
            moneyMaker.MakeMoney -= _data.Earn;
        }

        public void UnRegist(IMoneyUser moneyUser)
        {
            moneyUser.UseMoney -= _data.Use;
        }

        public void SetOnUseMoney(Action action, bool _isRegist)
        {
            if (_isRegist)
                _data.OnUse += action;
            else
                _data.OnUse -= action;
        }


        private class MoneyData
        {
            private Action _onEarn = null;
            public event Action OnEarn { add { _onEarn += value; } remove { _onEarn -= value; } }
            private Action<int> _onValueChanged = null;
            public event Action<int> OnValueChanged { add { _onValueChanged += value; } remove { _onValueChanged -= value; } }
            private Action _onUse = null;
            public event Action OnUse { add { _onUse += value; } remove { _onUse -= value; } }
            int _value = 0;
            public int Value { get { return _value; } }

            public int GetValue() { return _value; }
            public void Earn(int value)
            {
                _value += value;

                _onEarn?.Invoke();
                _onValueChanged?.Invoke(_value);
            }

            public void Use(int value)
            {
                _value -= value;
                _onUse?.Invoke();
                _onValueChanged?.Invoke(_value);
            }

            public void Reset(int value = 0)
            {
                _value = value;
                _onValueChanged?.Invoke(_value);
            }
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            List<MonoBehaviour> _allChild = mono.GetComponentsInChildren<MonoBehaviour>(true).ToList();
            List<MonoBehaviour> reader = new List<MonoBehaviour>();
            List<MonoBehaviour> maker = new List<MonoBehaviour>();
            List<MonoBehaviour> user = new List<MonoBehaviour>();

            for (int i = 0; i < _allChild.Count; i++)
            {
                if (_allChild[i] is IMoneyValueReader)
                    reader.Add(_allChild[i]);

                if (_allChild[i] is IMoneyMaker)
                    maker.Add(_allChild[i]);

                if (_allChild[i] is IMoneyUser)
                    user.Add(_allChild[i]);
            }

            _reader = reader.ToArray();
            _maker = maker.ToArray();
            _user = user.ToArray();
        }
#endif // UNITY_EDITOR
    }

    public interface IMoneyValueReader
    {
        event Func<int> GetMoneyValue;
    }

    public interface IMoneyMaker
    {
        event Action<int> MakeMoney;
    }

    public interface IMoneyUser
    {
        event Action<int> UseMoney;
    }
}

