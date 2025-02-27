using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class PlayerMediator : InitManagedBehaviorBase, ITileXpGetter, IDamageable
    {


        public bool IsCanUpdate = true;

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
        [SerializeField] UnitHammer _attacker;
        [SerializeField] Collider _col;
        [SerializeField] int _xp = 0;
        [SerializeField] int _combo;

        [Space]
        [Header("LevelUp")]
        [SerializeField] GameObject[] _hammers;
        [SerializeField] int[] _hammerLevels;
        int _levelIndex = 0;

        protected override void _Init()
        {
            ColDict.RegistData(_col, this);
            _moveHandler.Init();
            _attacker.RegistUser(this);
            _attacker.Init();
        }

        protected override void _Release()
        {
            _moveHandler.Release();
        }

        protected virtual void Update()
        {
            if (!IsCanUpdate)
                return;
            _moveHandler.UpdateMove();

            if (_animator == null)
                return;
        }

        public void GetXp(int xp)
        {
            _xp += xp;

            if (_levelIndex >= _hammerLevels.Length)
                return;

            if (_hammerLevels[_levelIndex] > _xp)
                return;

            _hammers[_levelIndex].SetActive(false);
            _levelIndex++;
            _hammers[_levelIndex].SetActive(true);
            _attacker.AddRange();
        }

        public void GetDamage()
        {
            Debug.Log("Ouch");
        }
    }
}