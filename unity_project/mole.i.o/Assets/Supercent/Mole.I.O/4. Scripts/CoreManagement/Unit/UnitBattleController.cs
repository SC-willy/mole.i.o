using System;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class UnitBattleController : IInitable, ITileXpGetter
    {
        const int ATTACKABLE_COMBO = 5;
        readonly private static int _animTrigHit = Animator.StringToHash("Hit");
        readonly private static int _animTrigDie = Animator.StringToHash("Die");
        readonly private static int _animTrigHitted = Animator.StringToHash("Ouch");
        readonly private static int _animIdle = Animator.StringToHash("Idle");

        public event Action<float> OnCombo;
        public event Action<float> OnSetSize;

        public int Level => _level;
        public int Xp => _xp;

        [SerializeField] HexHammer _hitter;
        [SerializeField] GameObject[] _hammers;
        [SerializeField] AnimEventContainer _animEvent;
        [SerializeField] Animator _animator = null;
        [SerializeField] Transform _attackTr;
        [SerializeField] LayerMask _mask;
        [SerializeField] GameObject[] _chargeObjs;
        [SerializeField] float _killRange = 3f;
        [SerializeField] int _level = 0;
        [SerializeField] int _xp = 0;
        LevelData.HammerLevel _nextLevelInfo;
        int _combo = 0;
        int _curHammerType = 0;

        public void Init()
        {
            _hitter.OnHit += PlayAttackAnim;
            _animEvent.OnAnimEvent += CheckEnemy;
            SetLevel(_level);
        }

        public void Release()
        {
            _hitter.OnHit -= PlayAttackAnim;
            _animEvent.OnAnimEvent -= CheckEnemy;
        }
        private void PlayAttackAnim() => _animator.SetTrigger(_animTrigHit);

        private void CheckEnemy()
        {
            _hitter.HitTile(this);

            if (_combo < ATTACKABLE_COMBO)
            {
                _combo++;
                OnCombo?.Invoke((float)_combo / ATTACKABLE_COMBO);

                if (_combo == ATTACKABLE_COMBO)
                    ReadyCharge();

                return;
            }

            Collider[] others = Physics.OverlapSphere(_attackTr.position, _killRange, _mask);

            for (int i = 0; i < others.Length; i++)
            {
                var enemy = ColDict.GetData(others[i]);

                if (enemy == null)
                    continue;

                enemy.GetDamage(_level);
                ReleaseCharge();
            }
        }

        private void ReadyCharge()
        {
            for (int i = 0; i < _chargeObjs.Length; i++)
            {
                _chargeObjs[i].SetActive(true);
            }
        }


        private void ReleaseCharge()
        {
            if (_combo <= 0)
                return;

            _combo = 0;
            for (int i = 0; i < _chargeObjs.Length; i++)
            {
                _chargeObjs[i].SetActive(false);
            }
            OnCombo?.Invoke(0);
        }

        public void GetXp(int xp)
        {
            _xp += xp;

            if (_level >= InGameManager.CurLevelData.MaxHammerLevel)
                return;

            if (_nextLevelInfo.RequireXp > _xp)
                return;

            SetNextLevel();
        }


        private void SetSize(float size) => OnSetSize?.Invoke(size);
        private void SetNextLevel()
        {
            _hammers[_curHammerType].SetActive(false);

            _hammers[_nextLevelInfo.HammerModelType].SetActive(true);
            SetRange(_nextLevelInfo.AttackRange);
            SetSize(_nextLevelInfo.PlayerSize);

            _level++;
            _nextLevelInfo = InGameManager.CurLevelData.GetNextHammerLvData(_level);
        }

        private void SetLevel(int level)
        {
            for (int i = 0; i < _hammers.Length; i++)
            {
                _hammers[i].SetActive(false);
            }

            _level = level;
            _nextLevelInfo = InGameManager.CurLevelData.GetNextHammerLvData(_level);
            _curHammerType = _nextLevelInfo.HammerModelType;

            SetNextLevel();
        }

        public void SetRange(int range) => _hitter.SetRange(range);

        public void GradeDown()
        {
            _animator.SetTrigger(_animTrigHitted);
            SetLevel(Mathf.Min(_level - 1, 0));
            SetRandomXp();
        }

        public void Die()
        {
            _animator.SetTrigger(_animTrigDie);
            _hitter.enabled = false;
        }

        public void ResetData(int xp = 0)
        {
            _animator.Play(_animIdle);
            _hitter.enabled = true;
            _xp = xp;
            SetLevel(InGameManager.CurLevelData.EvaluateXpToLevel(_xp));
        }

        public void SetRandomXp()
        {
            _xp = InGameManager.CurLevelData.EvaluateLevelToRandomXp(_level);
        }
    }
}