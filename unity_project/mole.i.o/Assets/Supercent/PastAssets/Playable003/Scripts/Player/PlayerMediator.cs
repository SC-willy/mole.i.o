using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class PlayerMediator : InitManagedBehaviorBase
    {
        readonly private static int _animBoolHit = Animator.StringToHash("Hit");

        [SerializeField] protected PlayerMoveHandler _moveHandler = new PlayerMoveHandler();
        [SerializeField] protected Animator _animator = null;
        [SerializeField] AnimEventContainer _animEvent;
        public bool IsCanUpdate = true;
        protected bool _wasMoved = false;
        [SerializeField] int _xp;
        [SerializeField] int[] _hammerLevels;
        [SerializeField] GameObject[] _hammers;
        [SerializeField] HexHammer _hitter;
        [SerializeField] Transform _attackTr;
        [SerializeField] LayerMask _mask;
        [SerializeField] float _killRange = 3f;
        int _levelIndex = 0;

        protected override void _Init()
        {
            _moveHandler.OnMove += ExecuteOnMove;
            _moveHandler.OnStop += ExecuteOnStop;
            _moveHandler.Init();

            _hitter.OnHit += PlayAttackAnim;
            _animEvent.OnAnimEvent += CheckEnemy;
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

        public void ExecuteOnMove()
        {
            if (_wasMoved)
                return;
            if (_animator == null)
                return;
            _wasMoved = true;
        }
        public void ExecuteOnStop()
        {
            if (!_wasMoved)
                return;
            if (_animator == null)
                return;
            _wasMoved = false;
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
            _hitter.AddRange();
        }

        private void PlayAttackAnim() => _animator.SetTrigger(_animBoolHit);

        private void CheckEnemy()
        {
            Collider[] others = Physics.OverlapSphere(_attackTr.position, _killRange, _mask);

            for (int i = 0; i < others.Length; i++)
            {
                var enemy = EnemyDict.GetData(others[i]);

                if (enemy == null)
                    continue;

                enemy.GetDamage();
            }

            _hitter.HitTile();
        }
    }
}