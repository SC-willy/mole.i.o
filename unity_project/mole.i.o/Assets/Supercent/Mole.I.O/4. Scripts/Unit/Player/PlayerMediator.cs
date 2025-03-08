using System;
using System.Collections;
using Supercent.MoleIO.Management;
using Supercent.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Supercent.MoleIO.InGame
{
    public class PlayerMediator : InitManagedBehaviorBase, IDamageable
    {
        const float LEVEL_UP_WAIT_TIME = 0.2f;
        const float LEVEL_UP_SPEED = 4f;
        private bool _isCanUpdate = false;

        readonly private static int _animTrigLevelUp = Animator.StringToHash("Up");

        public event Action OnLevelUp;

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

        Coroutine _coroutine;

        public int GetPlayerXp() => _attacker.Xp;
        public int GetPlayerGettedXp() => _attacker.Xp - PlayerData.SkillLevel1 * (int)GameManager.GetDynamicData(GameManager.EDynamicType.LevelPerUpgrade);
        public void StartUpdate()
        {
            _isCanUpdate = true;
            _attacker.ActiveAttack(true);
        }
        public UnitBattleController Attacker => _attacker;

        protected override void _Init()
        {
            ColDict.RegistData(_col, this);
            _moveHandler.Init();
            _attacker.Init();
            _attacker.OnSetSize += LevelUp;
            _attacker.OnEndStun += SetColOn;

            _attacker.SetPlayerUpgrade();
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
            _attacker.GetBumped();
        }

        public void GetDeadlyAttack()
        {
            _isCanUpdate = false;
            _col.enabled = false;
            _attacker.GetStun();
        }

        private void SetColOn()
        {
            _isCanUpdate = true;
            StartCoroutine(CoStopInvincible());
        }

        private IEnumerator CoStopInvincible()
        {
            yield return CoroutineUtil.WaitForSeconds(GameManager.GetDynamicData(GameManager.EDynamicType.InvincibleTime));
            _col.enabled = true;
        }

        private void LevelUp(float size)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _animator.SetTrigger(_animTrigLevelUp);
            _coroutine = StartCoroutine(CoStartGrow(size));
            OnLevelUp?.Invoke();
        }

        private IEnumerator CoStartGrow(float size)
        {
            yield return CoroutineUtil.WaitForSeconds(LEVEL_UP_WAIT_TIME);
            Vector3 start = transform.localScale;
            Vector3 end = Vector3.one * size;
            float lerpValue = 0;
            while (lerpValue < 1f)
            {
                lerpValue = Mathf.Min(lerpValue + Time.deltaTime * LEVEL_UP_SPEED, 1f);
                transform.localScale = Vector3.Lerp(start, end, lerpValue);
                yield return null;
            }
            _coroutine = null;

        }
    }
}
