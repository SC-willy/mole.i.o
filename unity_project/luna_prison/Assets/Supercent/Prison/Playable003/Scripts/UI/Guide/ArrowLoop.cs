namespace Supercent.PrisonLife.Playable003
{
    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public class ArrowLoop : InitManagedBehaviorBase
    {
        [SerializeField] Transform _startPoint;
        [SerializeField] Transform _endPoint;

        [SerializeField] LineRenderer _lineRenderer;

        Vector3 _currentEndPos = Vector3.zero;
        protected override void _Init()
        {
            SetEndTr();
        }

        public void Update()
        {
            if (_startPoint != null)
                _lineRenderer.SetPosition(0, _startPoint.position);
        }

        public void SetStartTr(Transform tr) => _startPoint = tr;
        public void SetEndTr(Transform tr)
        {
            _endPoint = tr;
            _currentEndPos = _endPoint.position;
            _lineRenderer.SetPosition(1, _currentEndPos);
        }
        public void SetEndTr()
        {
            if (_endPoint == null)
                return;

            _currentEndPos = _endPoint.position;
            _lineRenderer.SetPosition(1, _currentEndPos);
        }
        protected override void _Release()
        {
        }

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            base.OnBindSerializedField();
        }
#endif
    }
}