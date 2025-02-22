using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Supercent.PrisonLife.Playable003
{
    public class UnlockPad : BoolTrigger, IResetable, IMoneyValueReader, IMoneyUser
    {
        // Events 
        public event Func<int> GetMoneyValue { add { _payInfo.GetMoneyValue += value; } remove { _payInfo.GetMoneyValue -= value; } }
        public event Action<int> UseMoney { add { _payInfo.OnUseMoneyFunction += value; } remove { _payInfo.OnUseMoneyFunction -= value; } }
        public event Action OnGiveMoneyStart;
        public event Action OnGiveMoneyEnd;
        public event Action OnUpgrade;
        public event Action<int> OnUpgradeFunction;
        public event Action OnStackSound { add { _canvasInfo.OnStackSound += value; } remove { _canvasInfo.OnStackSound -= value; } }

        // public
        public Transform LastEnterTr => _lastEnterTr;
        public int UpgradeCode => _upgradeCode;
        public bool CanUpgrade = true;

        [Space(10f)]
        [SerializeField] int _upgradeCode;
        [SerializeField] UnlockPadPayHandler _payInfo = new UnlockPadPayHandler();
        [SerializeField] UnlockPadCanvas _canvasInfo = new UnlockPadCanvas();
        [SerializeField] UnlockPadAnimator _animatorInfo = new UnlockPadAnimator();
        Transform _lastEnterTr;

        private void Start()
        {
            _payInfo.OnLeftMoneyChanged += _canvasInfo.UpdateText;
            _payInfo.StartSetup();

            _payInfo.OnCompletePay += StartUnlock;
            _payInfo.OnGiveMoneyEnd += EndGiveMoney;
            _canvasInfo.UpdateText(_payInfo.LeftMoney);
        }

        private void OnDestroy()
        {
            _payInfo.OnLeftMoneyChanged -= _canvasInfo.UpdateText;
            _payInfo.OnCompletePay -= StartUnlock;
            _payInfo.OnGiveMoneyEnd -= EndGiveMoney;
        }

        private void Update()
        {
            if (_isEnter)
            {
                if (CanUpgrade)
                    _payInfo.UpdateManualy(Time.deltaTime);

                if (_payInfo.IsReady)
                    _canvasInfo.CheckInputSound();
            }

            _canvasInfo.UpdateGauge(_payInfo.ValueRatio);
        }
        private void EndGiveMoney()
        {
            _animatorInfo.IsEnter = false;
            OnGiveMoneyEnd?.Invoke();
        }

        private void StartUnlock()
        {
            CanUpgrade = false;
            StartCoroutine(WaitForUnlock());
        }

        IEnumerator WaitForUnlock()
        {
            yield return new WaitForSeconds(_payInfo.GetDurationTime);
            Unlock();
        }
        private void Unlock()
        {
            OnUpgrade?.Invoke();
            OnUpgradeFunction?.Invoke(_upgradeCode);

            if (_animatorInfo != null)
                _animatorInfo.RemoveNode();
            else
                gameObject.SetActive(false);

            OnGiveMoneyStart = null;
            return;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            _lastEnterTr = other.transform;
            _payInfo.ResetLastGivenTime();

            if (_payInfo.CanUseMoney)
            {
                _animatorInfo.IsEnter = true;
                OnGiveMoneyStart?.Invoke();
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            _animatorInfo.IsEnter = false;
            EndGiveMoney();
        }

        public void Reset()
        {
            _payInfo.Reset();
        }

        public void SetCost(int cost)
        {
            _payInfo.SetCost(cost);
        }

        public void ExitAreaManually() => _isEnter = false;


        // events
        public void SetOnUpgradeEvent(Action action, bool _isRegist)
        {
            if (_isRegist)
                OnUpgrade += action;
            else
                OnUpgrade -= action;
        }

        public void ReleaseLockPad()
        {
            _animatorInfo.Unlock();
            CanUpgrade = true;
        }
    }
}