using System;
using UnityEngine;


namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class UnlockPadPayHandler : IStartable, IUpdateable, IResetable
    {
        // Event
        public event Func<int> GetMoneyValue;
        public event Action<int> OnUseMoneyFunction;
        public event Action<int> OnLeftMoneyChanged;
        public event Action OnGiveMoneyEnd;
        public event Action OnCompletePay;

        // Public
        public float ValueRatio => Mathf.Lerp((_givenCost - _currentGivedMoney) * _inversedCost, _givenCost * _inversedCost, (Time.time - _realLastGivenTime) * _inversedDurationTime);
        public float GetDurationTime => _getDurationTime;
        public int LeftMoney => _leftMoney;
        public bool IsReady => _currentMoney > 0 && _givenCost < _cost;
        public bool CanUseMoney => GetMoneyValue != null && GetMoneyValue.Invoke() > 0;

        [Header("Time")]
        [SerializeField] float _getDurationTime = 0.05f;
        [SerializeField] float _getTotalTime = 0.7f;

        // CashedTiming
        float _inversedDurationTime;
        float _controlledLastGivenTime = 0f;
        float _realLastGivenTime = 0f;

        // Cost
        int _cost = 20;
        float _inversedCost = 0;
        int _givenCost = 0;

        // Money
        int _useMoneyAtOnceValue = 1;
        int _currentMoney;
        int _leftMoney;
        int _willGiveMoney;
        int _currentGivedMoney;

        public void StartSetup()
        {
            _inversedDurationTime = 1f / _getDurationTime;
            _inversedCost = 1f / (float)_cost;
        }

        public void UpdateManualy(float dt)
        {
            if (GetMoneyValue != null)
            {
                _currentMoney = GetMoneyValue.Invoke();
            }

            if (!IsReady)
                return;

            if (_controlledLastGivenTime + _getDurationTime <= Time.time)
            {
                _willGiveMoney = (int)((Time.time - _controlledLastGivenTime) * _inversedDurationTime);

                _controlledLastGivenTime += _willGiveMoney * _getDurationTime;
                _realLastGivenTime = Time.time;
                _willGiveMoney *= _useMoneyAtOnceValue;
                _leftMoney = _cost - _givenCost;

                if (_willGiveMoney <= _currentMoney)
                {
                    if (_willGiveMoney < _leftMoney)
                    {
                        OnUseMoneyFunction?.Invoke(_willGiveMoney);
                        _givenCost += _willGiveMoney;
                        _currentGivedMoney = _willGiveMoney;
                    }
                    else
                    {
                        OnUseMoneyFunction?.Invoke(_leftMoney);
                        _givenCost += _leftMoney;
                        _currentGivedMoney = _leftMoney;
                    }
                }
                else
                {
                    if (_currentMoney < _leftMoney)
                    {
                        OnUseMoneyFunction?.Invoke(_currentMoney);
                        _givenCost += _currentMoney;
                        _currentGivedMoney = _currentMoney;
                    }
                    else
                    {
                        OnUseMoneyFunction?.Invoke(_leftMoney);
                        _givenCost += _leftMoney;
                        _currentGivedMoney = _leftMoney;
                    }
                }

                _leftMoney = _cost - _givenCost;
                OnLeftMoneyChanged.Invoke(_leftMoney);

                if (_givenCost >= _cost)
                {
                    OnCompletePay?.Invoke();
                    OnGiveMoneyEnd?.Invoke();
                }
                else if (GetMoneyValue != null && GetMoneyValue.Invoke() == 0)
                {
                    OnGiveMoneyEnd?.Invoke();
                }
            }
        }

        public void ResetLastGivenTime() => _controlledLastGivenTime = Time.time;
        public void SetCost(int cost)
        {
            _cost = cost;
            _inversedDurationTime = 1f / _getDurationTime;
            _inversedCost = 1 / _cost;
            OnLeftMoneyChanged?.Invoke(_cost);

            _givenCost = 0;
            _leftMoney = _cost;
            _useMoneyAtOnceValue = Mathf.Max((int)(_cost / (_getTotalTime * _inversedDurationTime)), 1);
        }

        public void Reset()
        {
            SetCost(_cost);
        }

    }
}