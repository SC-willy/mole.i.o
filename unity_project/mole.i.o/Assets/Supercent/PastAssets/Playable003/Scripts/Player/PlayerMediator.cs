using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class PlayerMediator : InitManagedBehaviorBase
    {
        private const float ANIM_TRANSITION_SPEED = 5f;
        readonly private static int _animFloatMove = Animator.StringToHash("IsMove");

        [SerializeField] protected PlayerMoveHandler _moveHandler = new PlayerMoveHandler();
        [SerializeField] protected Animator _animator = null;
        float _animFloat = 0;
        float _animChangeValue = -1;
        public bool IsCanUpdate = true;
        protected bool _wasMoved = false;
        [SerializeField] int _xp;
        [SerializeField] int[] _hammerLevels;
        [SerializeField] GameObject[] _hammers;
        [SerializeField] HexHitterHammer _hitter;
        [SerializeField] Transform _attackTr;
        [SerializeField] LayerMask _mask;
        [SerializeField] float _killRange = 3f;
        int _levelIndex = 0;

        protected override void _Init()
        {
            _moveHandler.OnMove += ExecuteOnMove;
            _moveHandler.OnStop += ExecuteOnStop;
            _moveHandler.Init();

            _hitter.OnHit += CheckEnemy;
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

            _animFloat = Mathf.Clamp01(_animFloat + (Time.deltaTime * _animChangeValue * ANIM_TRANSITION_SPEED));
            _animator.SetFloat(_animFloatMove, _animFloat);
        }

        public void ExecuteOnMove()
        {
            if (_wasMoved)
                return;
            if (_animator == null)
                return;
            _wasMoved = true;
            _animChangeValue = 1;
        }
        public void ExecuteOnStop()
        {
            if (!_wasMoved)
                return;
            if (_animator == null)
                return;
            _wasMoved = false;
            _animChangeValue = -1;
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
        }
    }
}