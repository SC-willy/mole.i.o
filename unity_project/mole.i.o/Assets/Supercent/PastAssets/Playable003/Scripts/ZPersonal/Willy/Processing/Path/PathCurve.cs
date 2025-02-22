using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class PathCurve : PathBase
    {
        [SerializeField] Transform _startTr;
        [SerializeField] Transform _middleTr;
        [SerializeField] Transform _endTr;
        [SerializeField] AnimationCurve _curve;
        private Vector3 _currentForward;
        public override Vector3 Evaluate(float t)
        {
            Vector3 currentPos = Vector3.Lerp(
                Vector3.Lerp(_startTr.position, _middleTr.position, t),
                Vector3.Lerp(_middleTr.position, _endTr.position, t),
                _curve.Evaluate(t));
            Vector3 nextPos = Vector3.Lerp(
                            Vector3.Lerp(_startTr.position, _middleTr.position, t),
                            Vector3.Lerp(_middleTr.position, _endTr.position, t),
                            _curve.Evaluate(t - 0.01f));
            _currentForward = (nextPos - currentPos).normalized;

            return currentPos;
        }

        public override Vector3 GetForwardAngle()
        {
            return _currentForward;
        }

        public override void OnDrawGizmos()
        {
            if (_startTr == null || _middleTr == null || _endTr == null || _curve == null)
                return;

            Gizmos.color = Color.green;

            int segments = 20; //곡선 감도(많을수록 정확함)
            Vector3 previousPoint = _startTr.position;

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 point = Evaluate(t);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }

            //기준선 그림
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(_startTr.position, _middleTr.position);
            Gizmos.DrawLine(_middleTr.position, _endTr.position);
        }
    }
}
