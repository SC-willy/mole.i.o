using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class TransTokenJump : TransTokenStartTrRot
    {
        public Vector3 Gap;
        public Vector3 StartPos => _startPos;
        private Vector3 _startPos;

        public float StartY;
        public float BottomY;
        public float MaxY;

        float _startHeight;
        float _bottomHeight;
        float _stackMaxHeight;

        protected override void _Initialize()
        {
            base._Initialize();
            _startPos = _target.position;
            _startHeight = Start.position.y;
            _bottomHeight = End.position.y + Gap.y;
            _stackMaxHeight = Mathf.Max(_startHeight, _bottomHeight);
        }

    }
}