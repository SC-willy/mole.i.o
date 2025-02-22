using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class PathMultiLine : PathBase
    {
        [SerializeField] Transform[] _transforms;

        private Vector3 _currentForward;
        float[] distances;
        float totalDistance = 0f;

        private void Awake()
        {
            distances = new float[_transforms.Length - 1];

            for (int i = 0; i < _transforms.Length - 1; i++)
            {
                distances[i] = Vector3.Distance(_transforms[i].position, _transforms[i + 1].position);
                totalDistance += distances[i];
            }
        }
        public override Vector3 Evaluate(float t)
        {
            if (_transforms.Length < 2)
            {
                Debug.LogWarning("PathMultiLine needs More then 2 paths!");
                return Vector3.zero;
            }

            float targetDistance = t * totalDistance;

            float cumulativeDistance = 0f;
            int segmentIndex = 0;

            for (int i = 0; i < distances.Length; i++)
            {
                if (cumulativeDistance + distances[i] >= targetDistance)
                {
                    segmentIndex = i;
                    break;
                }
                cumulativeDistance += distances[i];
            }

            float segmentT = (targetDistance - cumulativeDistance) / distances[segmentIndex];
            _currentForward = _transforms[segmentIndex].forward;
            return Vector3.Lerp(_transforms[segmentIndex].position, _transforms[segmentIndex + 1].position, segmentT);
        }

        public override void OnDrawGizmos()
        {
            if (_transforms.Length < 2)
                return;
            Gizmos.color = Color.green;
            for (int i = 0; i < _transforms.Length - 1; i++)
            {
                Gizmos.DrawLine(_transforms[i].position, _transforms[i + 1].position);
            }
        }

        public override Vector3 GetForwardAngle()
        {
            return _currentForward;
        }
    }
}
