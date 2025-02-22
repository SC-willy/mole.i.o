using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class TransTokenStartTr : TransTokenStartTrPos
    {
        public Quaternion StartRot => _startRot;
        private Quaternion _startRot;
        protected override void _Initialize()
        {
            base._Initialize();
            _startRot = _target.rotation;
        }
    }
}