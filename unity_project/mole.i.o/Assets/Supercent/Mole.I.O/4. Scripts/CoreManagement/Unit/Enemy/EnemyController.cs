using System;
using TMPro;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class EnemyController : InitManagedBehaviorBase, IDamageable
    {
        public Action<EnemyController> OnHit;
        public Action<EnemyController> OnDie;

        public bool IsDie => _isDie;


        [CustomColor(0, 0, 0.2f)]
        [SerializeField] UnitBattleController _attacker;
        [SerializeField] Transform _model;
        [SerializeField] Collider _col;
        [SerializeField] float _speed;
        [SerializeField] float _rotateSpeed;
        [SerializeField] float _rotateDuration;
        Transform _followTarget;
        Vector3 _offset;
        float _rotateLerpValue = 0;
        float _lastRotateTime = 0;

        bool _isDie = true;
        bool _isRotate = false;

        public UnitBattleController BattleController => _attacker;


        protected override void _Init()
        {
            ColDict.RegistData(_col, this);
            Vector2 rand = UnityEngine.Random.insideUnitCircle;
            _offset.x = rand.x;
            _offset.z = rand.y;

            _attacker.OnSetSize += SetSize;
            _attacker.Init();
            _attacker.SetRandomXp();
        }

        public void ActiveBattle(bool on)
        {
            _attacker.ActiveAttack(on);
            _isDie = !on;

            if (on)
            {
                _attacker.SetAiInfo();
                _speed = GameManager.GetDynamicData(GameManager.EDynamicType.AiSpeed);
            }
        }

        protected override void _Release()
        {
            _attacker.OnSetSize -= SetSize;
        }

        public void SetTarget(Transform tr)
        {
            _followTarget = tr;
        }

        private void Update()
        {
            if (_isDie)
                return;

            transform.position += _model.forward * Time.deltaTime * _speed;

            if (_lastRotateTime + _rotateDuration > Time.deltaTime)
                StartRotate();

            if (!_isRotate)
                return;

            _rotateLerpValue = Mathf.Min(_rotateLerpValue + Time.deltaTime * _rotateSpeed, 1f);
            _model.forward = Vector3.Lerp(_model.forward, (_followTarget.position + _offset - transform.position).normalized, _rotateLerpValue);

            if (_rotateLerpValue == 1f)
            {
                _isRotate = false;
            }
        }

        private void StartRotate()
        {
            _lastRotateTime = Time.time;
            _rotateLerpValue = 0;
            _isRotate = true;
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
            OnHit.Invoke(this);
        }

        public void GetDeadlyAttack()
        {
            _attacker.Die();
            _isDie = true;
            OnDie?.Invoke(this);
        }

        private void SetSize(float size)
        {
            transform.localScale = Vector3.one * size;
        }

        public EnemyController Respawn(int xp)
        {
            gameObject.SetActive(true);
            _attacker.ResetData(xp);
            _isDie = false;
            return this;
        }

        public bool CheckIsDead() => _isDie;
    }
}