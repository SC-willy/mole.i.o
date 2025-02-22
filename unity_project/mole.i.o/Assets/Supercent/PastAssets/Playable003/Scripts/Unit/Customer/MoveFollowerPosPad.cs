using System;
using Supercent.Util;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class MoveFollowerPosPad : BehaviourBase
    {
        public enum EPosType
        {
            Default,
            EscalatorUp,
            EscalatorDown,
            Elevator,
        }
        [SerializeField] float _padding;
        [SerializeField] Transform[] _transforms;
        public Transform[] Transforms => _transforms;
        [SerializeField] PosInfo[] _formations;
        [SerializeField] Transform _target;
        Vector3 _dir;

        public bool _isMoving;
        public void SetMove(bool isMove = true)
        {
            _isMoving = isMove;
        }
        private void Start()
        {
            ChangeForm(EPosType.Default);
        }

        private void FixedUpdate()
        {
            if (!_isMoving)
                return;

            _dir = transform.position - _target.position;
            _dir.y = 0;
            if (_dir.sqrMagnitude > _padding)
            {
                transform.position = _target.position + _dir.normalized * _padding;
            }
        }

        public void ChangeForm(EPosType type)
        {
            for (int i = 0; i < _formations.Length; i++)
            {
                PosInfo curForm = _formations[i];
                if (curForm.Type != type)
                    continue;

                for (int j = 0; j < _transforms.Length; j++)
                {
                    _transforms[j].localPosition = curForm.Formations[j];
                }
                break;
            }
        }

        [Serializable]
        private class PosInfo
        {
            [SerializeField] public Vector3[] Formations;
            [SerializeField] public EPosType Type;
        }

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            base.OnBindSerializedField();
            if (_formations == null)
                _formations = new PosInfo[1];
            if (_formations.Length <= 0)
                _formations = new PosInfo[1];

            PosInfo firstPos = _formations[0];
            firstPos.Formations = new Vector3[_transforms.Length];
            for (int i = 0; i < _transforms.Length; i++)
            {
                firstPos.Formations[i] = _transforms[i].localPosition;
            }
        }
#endif // UNITY_EDITOR
    }
}