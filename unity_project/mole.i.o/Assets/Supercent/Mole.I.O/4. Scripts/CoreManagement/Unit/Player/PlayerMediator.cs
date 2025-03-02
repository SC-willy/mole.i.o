using System;
using UnityEngine;
using UnityEngine.UI;

namespace Supercent.MoleIO.InGame
{
    public class PlayerMediator : InitManagedBehaviorBase, IDamageable
    {
        public event Action OnDie;
        private bool _isCanUpdate = false;

        [Header("Functions")]
        [CustomColor(0.2f, 0, 0)]
        [SerializeField] PlayerMoveHandler _moveHandler = new PlayerMoveHandler();

        [Space]
        [Header("Animation")]
        [SerializeField] AnimEventContainer _animEvent;
        [SerializeField] Animator _animator = null;

        [Space]
        [Header("Attack")]
        [CustomColor(0, 0.2f, 0)]
        [SerializeField] UnitBattleController _attacker;
        [SerializeField] Collider _col;
        [SerializeField] Image _gauge;
        [SerializeField] int _combo = 0;

        public int GetPlayerXp() => _attacker.Xp;
        public void StartUpdate() => _isCanUpdate = true;
        public UnitBattleController Attacker => _attacker;

        protected override void _Init()
        {
            ColDict.RegistData(_col, this);
            _moveHandler.Init();
            _attacker.Init();
            _attacker.OnCombo += UpdateComboUI;
            _attacker.OnSetSize += SetSize;
        }

        protected override void _Release()
        {
            _isCanUpdate = false;
            _moveHandler.Release();
            _col.enabled = false;
        }

        protected virtual void Update()
        {
            if (!_isCanUpdate)
                return;
            _moveHandler.UpdateMove();

            if (_animator == null)
                return;
        }
        public void GetDamage(int attackerLevel)
        {
            if (_attacker.Level > attackerLevel)
                GetWeakAttack();
            else
                GetDeadlyAttack();
        }

        public void GetWeakAttack()
        {
            _attacker.GradeDown();
        }

        public void GetDeadlyAttack()
        {
            OnDie?.Invoke();
            _attacker.Die();
            _isCanUpdate = false;
        }

        public void UpdateComboUI(float comboValue)
        {
            if (_gauge != null)
                _gauge.fillAmount = comboValue;
        }

        private void SetSize(float size)
        {
            transform.localScale = Vector3.one * size;
        }

    }
}
