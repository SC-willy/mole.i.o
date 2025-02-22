using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class PathLine : PathBase
    {
        [SerializeField] Transform _startTr;
        [SerializeField] Transform _endTr;
        public override Vector3 Evaluate(float t)
        {
            return Vector3.Lerp(_startTr.position, _endTr.position, t);
        }

        public override Vector3 GetForwardAngle()
        {
            return _startTr.forward;
        }

        public override void OnDrawGizmos()
        {
            if (_startTr == null || _endTr == null)
                return;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_startTr.position, _endTr.position);
        }
    }

}
