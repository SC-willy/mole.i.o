using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class TransTokenStartTrPos : TransitionToken
    {
        public Vector3 StartPos => _startPos;
        private Vector3 _startPos;
        protected override void _Initialize()
        {
            base._Initialize();
            _startPos = _target.position;
        }
        public void FixStartPos(Vector3 pos)
        {
            _startPos = pos;
        }
    }
}