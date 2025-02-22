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

        protected override void _Init()
        {
            _moveHandler.OnMove += ExecuteOnMove;
            _moveHandler.OnStop += ExecuteOnStop;
            _moveHandler.Init();
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
    }
}