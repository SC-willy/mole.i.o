
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class TransTokenScatter : TransTokenStartTrPos
    {
        Vector3 _scatterPos;
        Vector3 _originForward;
        Vector3 _endForward;

        public Vector3 ScatterPos => _scatterPos;
        public Vector3 OriginForward => _originForward;
        public Vector3 EndForward => _endForward;
        protected override void _Initialize()
        {
            base._Initialize();

            _originForward = _target.forward;
            _endForward = Random.onUnitSphere;
        }

        public void SetScatterPos(float range)
        {
            _scatterPos = Random.insideUnitSphere;
            _scatterPos.y = 0;
            _scatterPos = (_scatterPos.normalized * range) + StartPos;
        }
    }
}