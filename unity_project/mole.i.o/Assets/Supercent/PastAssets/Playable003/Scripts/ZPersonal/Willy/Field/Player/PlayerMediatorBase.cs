using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class PlayerMediatorBase : InitManagedBehaviorBase
    {
        readonly private static int _animBoolMove = Animator.StringToHash("IsMoving");

        [SerializeField] protected PlayerMoveHandler _moveHandler = new PlayerMoveHandler();
        [SerializeField] protected Animator _animator = null;
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
        }

        public void ExecuteOnMove()
        {
            if (_wasMoved)
                return;
            if (_animator == null)
                return;
            _animator.SetBool(_animBoolMove, true);
            _wasMoved = true;
        }
        public void ExecuteOnStop()
        {
            if (!_wasMoved)
                return;
            if (_animator == null)
                return;
            _animator.SetBool(_animBoolMove, false);
            _wasMoved = false;
        }
    }
}