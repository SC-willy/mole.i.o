using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class UnitMover
    {
        [SerializeField] protected Transform _playerTr;
        [SerializeField] protected Transform _rotateModelTr;
        [SerializeField] protected float _moveSpeed = 6f;

        [HideInInspector] public bool IsCanMove = false;

        public void SetMoveSpeed(float newSpeed) => _moveSpeed = newSpeed;

        public virtual void UpdateMove(Vector3 dir)
        {
            if (!IsCanMove)
                return;
            if (dir == Vector3.zero)
                return;

            MoveTowards(dir);
            Rotate(dir);
        }

        public Vector3 MoveTowards(Vector3 dir)
        {
            _playerTr.position = Vector3.MoveTowards(_playerTr.position, _playerTr.position + dir, _moveSpeed * Time.deltaTime);
            return _playerTr.position;
        }

        public void Rotate(Vector3 dir)
        {
            _rotateModelTr.rotation = Quaternion.LookRotation(dir);
        }
    }
}