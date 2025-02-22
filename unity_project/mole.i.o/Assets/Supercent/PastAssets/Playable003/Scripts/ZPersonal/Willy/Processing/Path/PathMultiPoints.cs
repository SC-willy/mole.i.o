using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class PathMultiPoints : PathBase
    {
        [Header("Need +1 Transforms")]
        [SerializeField] Transform[] _transforms;
        private Vector3 _currentForward;

        public override Vector3 Evaluate(float t)
        {
            Transform currentTransform = _transforms[(int)(t * _transforms.Length)];
            _currentForward = currentTransform.forward;
            return currentTransform.position;
        }

        public override Vector3 GetForwardAngle()
        {
            return _currentForward;
        }

        public override void OnDrawGizmos()
        {
            if (_transforms.Length < 1)
                return;
            Gizmos.color = Color.green;
            for (int i = 0; i < _transforms.Length - 1; i++)
            {
                if (_transforms[i] == null)
                    return;
                Gizmos.DrawLine(_transforms[i].position, _transforms[i + 1].position);
            }
        }
    }
}
