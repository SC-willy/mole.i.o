using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class LobbyPlayerMoveHandler : IInitable
    {
        // Events
        public event Action OnMove;
        public event Action OnStop;

        public Vector3 ForwardDir => _forwardDir;

        [SerializeField] UnitRaycastMover _mover = new UnitRaycastMover();
        [SerializeField] float _moveSpeed = 6;
        Vector3 _forwardDir = Vector3.zero;
        float _mainCamY = 0;
        public void Init()
        {
            ScreenInputController.OnDragEvent += MovePlayer;
            ScreenInputController.OnPointerUpEvent += StopPlayer;
            ScreenInputController.OnPointerDownEvent += StopPlayer;
            _mainCamY = Camera.main.transform.rotation.eulerAngles.y;

            _mover.SetMoveSpeed(_moveSpeed);
            ResetCamY();
        }

        public void Release()
        {
            ScreenInputController.OnDragEvent -= MovePlayer;
            ScreenInputController.OnPointerUpEvent -= StopPlayer;
            ScreenInputController.OnPointerDownEvent -= StopPlayer;
        }

        public void ResetCamY(float mainCamY) => _mainCamY = mainCamY;
        public void ResetCamY() => ResetCamY(Camera.main.transform.rotation.eulerAngles.y);

        public void UpdateMove()
        {

            if (ScreenInputController.Direction == Vector2.zero)
            {
                StopPlayer();
                return;
            }
            _forwardDir = Quaternion.Euler(0, Mathf.Atan2(ScreenInputController.X, ScreenInputController.Y) * Mathf.Rad2Deg + _mainCamY, 0f) * Vector3.forward;
            _mover.UpdateMove(_forwardDir);
        }

        public void MovePlayer()
        {
            if (_mover.IsCanMove)
                return;

            _mover.IsCanMove = true;

            OnMove?.Invoke();
        }

        public void StopPlayer()
        {
            if (!_mover.IsCanMove)
                return;

            _mover.IsCanMove = false;

            OnStop?.Invoke();
        }

        public void AddMoveSpeed(float speed)
        {
            _moveSpeed += speed;
            _mover.SetMoveSpeed(_moveSpeed);
        }


    }
}