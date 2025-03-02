using System;
using System.Collections;
using Supercent.Util;
using UnityEngine;
using UnityEngine.UI;
namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class BossFlowManager : IInitable, IUpdateable
    {
        const float PHASE_2 = 0.5f;
        const float PHASE_3 = 1f;

        private static readonly int _animTriggerCharge = Animator.StringToHash("Charge");
        private static readonly int _animTriggerDie = Animator.StringToHash("Die");
        public event Action OnWin;
        public event Action OnFail;
        [SerializeField] MonoBehaviour _coroutieOwner;
        [SerializeField] GameObject _bossFlowObjs;
        [SerializeField] GameObject _bossFlowUi;
        [SerializeField] ImageTouchChecker _btn;
        [SerializeField] Image _gauge;
        [SerializeField] Animator _playerAnim;
        [SerializeField] int _requiredPower;
        [SerializeField] float _chargeTime;
        [SerializeField] float _resultDelay;

        float _inversedMaxPower = 0;
        float _chargeSpeed = 0;
        float _energy = 0;
        float _usedEnergy = 0;
        float _usedValue = 0;
        int _phase = 0;
        bool _isUpdate = false;
        bool _isChargeable = false;

        public void Init()
        {
            _btn.OnPointerDownEvent += StartCharge;
            _btn.OnPointerUpEvent += EndCharge;
        }

        public void Release()
        {
            _btn.OnPointerDownEvent -= StartCharge;
            _btn.OnPointerUpEvent -= EndCharge;
        }

        private void StartCharge() => _isUpdate = true;
        private void EndCharge() => _isUpdate = false;

        public void StartFlow(int energy)
        {
            _energy = energy;
            _gauge.fillAmount = 0;
            _usedEnergy = 0;
            _chargeSpeed = 1f / _chargeTime;
            _inversedMaxPower = 1f / _requiredPower;
            _phase = 0;
            _isChargeable = true;
            _bossFlowObjs.SetActive(true);
        }
        private void PlayWinFlow()
        {
            OnWin?.Invoke();
            _coroutieOwner.StartCoroutine(CoOpenWinUI());
            _bossFlowUi.gameObject.SetActive(false);
        }

        private void PlayFailFlow()
        {
            OnFail?.Invoke();
            _coroutieOwner.StartCoroutine(CoOpenFailUI());
        }

        private IEnumerator CoOpenWinUI()
        {
            yield return CoroutineUtil.WaitForSeconds(_resultDelay);
            _playerAnim.SetTrigger(_animTriggerCharge);
        }

        private IEnumerator CoOpenFailUI()
        {
            yield return CoroutineUtil.WaitForSeconds(_resultDelay);
            _playerAnim.SetTrigger(_animTriggerDie);
        }

        public void UpdateManualy(float dt)
        {
            if (!_isUpdate)
                return;

            if (!_isChargeable)
                return;

            _usedValue = _requiredPower * _chargeSpeed * dt;

            if (_usedValue > _energy)
            {
                _usedValue = _energy;
                if (_usedEnergy + _usedValue < _requiredPower)
                {
                    PlayFailFlow();
                    _isChargeable = false;
                    return;
                }
            }


            _usedEnergy += _usedValue;
            _energy -= _usedValue;
            _gauge.fillAmount = Mathf.Min(_usedEnergy * _inversedMaxPower, 1f);

            switch (_phase)
            {
                case 0:
                    _playerAnim.SetTrigger(_animTriggerCharge);
                    _phase++;
                    break;
                case 1:
                    if (_gauge.fillAmount < PHASE_2)
                        break;

                    _playerAnim.SetTrigger(_animTriggerCharge);
                    _phase++;
                    break;
                case 2:
                    if (_gauge.fillAmount < PHASE_3)
                        break;
                    _playerAnim.SetTrigger(_animTriggerCharge);
                    _phase++;
                    PlayWinFlow();
                    break;
            }
        }
    }

}
