using System;
using UnityEngine;
using UnityEngine.UI;

namespace Supercent.MoleIO.InGame
{
    public class LobyPlayerMediator : InitManagedBehaviorBase
    {
        readonly private static int _animBoolMove = Animator.StringToHash("Move");

        [SerializeField] LobbyPlayerMoveHandler _moveHandler = new LobbyPlayerMoveHandler();
        [SerializeField] Animator _animator = null;
        [SerializeField] Collider _col = null;
        bool _isCanUpdate = true;
        bool _wasMoved = false;

        public void StartUpdate() => _isCanUpdate = true;

        protected override void _Init()
        {
            _moveHandler.OnMove += ExecuteOnMove;
            _moveHandler.OnStop += ExecuteOnStop;
            _moveHandler.Init();
            _animator.SetBool(_animBoolMove, false);
            _col.enabled = false;
        }

        protected override void _Release()
        {
            _moveHandler.Release();
        }

        protected virtual void Update()
        {
            if (!_isCanUpdate)
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
            _col.enabled = false;
            _wasMoved = true;
        }
        public void ExecuteOnStop()
        {
            if (!_wasMoved)
                return;
            if (_animator == null)
                return;
            _animator.SetBool(_animBoolMove, false);
            _col.enabled = true;
            _wasMoved = false;
        }
    }
}
