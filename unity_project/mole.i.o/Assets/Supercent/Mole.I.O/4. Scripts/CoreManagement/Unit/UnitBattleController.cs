using System;
using System.Text;
using Supercent.MoleIO.Management;
using TMPro;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class UnitBattleController : IInitable, ITileXpGetter
    {
        const int ATTACKABLE_COMBO = 5;
        const int MAX_REDUCED_ATTACKRATE = 1;
        const float REDUCED_ATTACKRATE_VALUE = 0.99f;
        readonly private static int _animTrigHit = Animator.StringToHash("Hit");
        readonly private static int _animTrigDie = Animator.StringToHash("Die");
        readonly private static int _animTrigHitted = Animator.StringToHash("Ouch");
        readonly private static int _animIdle = Animator.StringToHash("Idle");
        readonly private static int _animMoveBool = Animator.StringToHash("Move");

        public event Action<float> OnCombo;
        public event Action<float> OnSetSize;
        public event Action OnChangeXp;
        public event Action<int> OnKill;

        public int Level => _level;
        public int Xp => _xp;
        public int PlayerCode => _hitter.PlayerCode;
        public void SetCode(int Code) => _hitter.SetPlayerCode(Code);

        [SerializeField] TMP_Text _name;
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
        int _killCount = 0;

        [Space]
        [Header("UI")]
        [SerializeField] TMP_Text _levelText;
        StringBuilder _stringBuilder = new StringBuilder();

        public void Init()
        {
            _hitter.OnHit += PlayAttackAnim;
            _animEvent.OnAnimEvent += CheckEnemy;
            SetLevel(_level);
        }

        public void ActiveAttack(bool on)
        {
            _animator.SetBool(_animMoveBool, on);
            _hitter.ActiveAttack(on);
        }
        public void SetPlayerUpgrade()
        {
            _xp += (PlayerData.SkillLevel1 - 1) * (int)GameManager.GetDynamicData(DynamicGameData.EDynamicType.LevelPerUpgrade);
            float reduceRate = MAX_REDUCED_ATTACKRATE;
            for (int i = 0; i < PlayerData.SkillLevel2 - 1; i++)
            {
                reduceRate *= REDUCED_ATTACKRATE_VALUE;
            }
            _hitter.ReduceHitDuration(1 - reduceRate);
        }

        public void SetName(string name)
        {
            _name.text = name;
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

                if (enemy.CheckIsDead())
                {
                    _killCount++;
                    OnKill?.Invoke(_killCount);
                }

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
            UpdateXpUI();

            if (_level >= InGameManager.CurLevelData.MaxHammerLevel)
                return;

            if (_nextLevelInfo.RequireXp > _xp)
                return;

            SetNextLevel();
        }

        private void SetNextLevel()
        {
            _hammers[_curHammerType].SetActive(false);

            _hammers[_nextLevelInfo.HammerModelType].SetActive(true);
            SetRange(_nextLevelInfo.AttackRange);
            OnSetSize?.Invoke(_nextLevelInfo.PlayerSize);

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
            UpdateXpUI();
        }

        public void SetRandomXp()
        {
            _xp = InGameManager.CurLevelData.EvaluateLevelToRandomXp(_level);
            UpdateXpUI();
        }

        private void UpdateXpUI()
        {
            OnChangeXp?.Invoke();

            if (_levelText == null)
                return;

            _stringBuilder.Clear();
            _stringBuilder.Append(_xp);
            _levelText.text = _stringBuilder.ToString();
        }
    }
}