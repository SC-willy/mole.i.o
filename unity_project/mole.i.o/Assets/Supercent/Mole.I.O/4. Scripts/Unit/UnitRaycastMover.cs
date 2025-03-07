using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class UnitRaycastMover : UnitMover
    {
        [SerializeField] float _rayRadius = 0.5f;
        [SerializeField] LayerMask _layerMask;

        public override void UpdateMove(Vector3 dir)
        {
            TryMoveTowards(dir);
            Rotate(dir);
        }
        public bool TryMoveTowards(Vector3 dir)
        {
            float dist = _moveSpeed * Time.deltaTime + _rayRadius;

            if (Physics.Raycast(_playerTr.position + Vector3.up, dir, out RaycastHit hit, dist, _layerMask))
            {
                Vector3 wallNormal = hit.normal;
                Vector3 slideDirection = Vector3.Cross(wallNormal, Vector3.up);
                float slideScalar = Vector3.Dot(slideDirection, dir);

                if (slideScalar < 0)
                {
                    slideDirection *= -1;
                    slideScalar *= -1;
                }

                dist -= hit.distance;
                if (Physics.Raycast(hit.point, slideDirection, dist, _layerMask))
                    return false;

                _playerTr.position =
                Vector3.MoveTowards(_playerTr.position,
                                    _playerTr.position + slideDirection,
                                    _moveSpeed * slideScalar * Time.deltaTime);
                return true;
            }
            _playerTr.position =
            Vector3.MoveTowards(_playerTr.position,
                                _playerTr.position + dir,
                                _moveSpeed * dir.magnitude * Time.deltaTime);
            return true;
        }
    }
}