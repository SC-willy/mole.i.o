
using Supercent.Util;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class UnitPatroller : BehaviourBase
    {
        readonly private static int _animBoolMove = Animator.StringToHash("IsMoving");

        [Header("Default")]
        [SerializeField] Animator _animator;
        [SerializeField] float _speed;
        [SerializeField] float _rotSpeed;
        [SerializeField] float _minTime;
        [SerializeField] float _maxTime;

        [Space(10f)]
        [Header("Patrol Info")]
        [SerializeField] int _patrolPos = 3;
        [SerializeField] Vector3 _minPos;
        [SerializeField] Vector3 _maxPos;

        [Space(10f)]
        [Header("Baked Info")]
        [SerializeField] Vector3[] _pos;

        Vector3 _currentDestination = Vector3.zero;
        float _nextPatrolTime = 0;
        int _posIndex = 0;
        bool _isMove = false;

        private void OnEnable()
        {
            _nextPatrolTime = Time.time - _maxTime;

            if (_pos != null && _pos.Length > _patrolPos)
                return;

            _pos = new Vector3[_patrolPos];
            for (int i = 0; i < _patrolPos; i++)
            {
                _pos[i] = new Vector3(Random.Range(_minPos.x, _maxPos.x), _minPos.y, Random.Range(_minPos.z, _maxPos.z));
            }
        }
        private void Update()
        {
            if (_nextPatrolTime < Time.time)
            {
                _nextPatrolTime = Time.time + Random.Range(_minTime, _maxTime);
                if (!_isMove)
                    ChangePos();
                else
                {
                    if (_animator != null)
                        _animator.SetBool(_animBoolMove, false);

                    _isMove = false;
                }
            }

            if (!_isMove)
                return;

            transform.position += transform.forward * _speed * Time.deltaTime;
            transform.forward += Vector3.Lerp(transform.forward, _currentDestination - transform.position, Time.deltaTime * _rotSpeed);
        }

        private void ChangePos()
        {
            _posIndex = (_posIndex + 1) % _pos.Length;
            _currentDestination = _pos[_posIndex];
            _isMove = true;
            if (_animator != null)
                _animator.SetBool(_animBoolMove, true);
        }

        public void SetAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void SetPos(Vector3[] pos) => _pos = pos;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _pos.Length; i++)
            {
                Gizmos.DrawSphere(_pos[i], 0.1f);
            }
        }

        protected override void OnBindSerializedField()
        {
            _pos = new Vector3[_patrolPos];
            for (int i = 0; i < _patrolPos; i++)
            {
                _pos[i] = new Vector3(Random.Range(_minPos.x, _maxPos.x), Random.Range(_minPos.y, _maxPos.y), Random.Range(_minPos.z, _maxPos.z));
            }
        }
#endif // UNITY_EDITOR
    }
}