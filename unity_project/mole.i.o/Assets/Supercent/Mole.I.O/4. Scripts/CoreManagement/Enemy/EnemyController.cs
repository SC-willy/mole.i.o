using System;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class EnemyController : MonoBehaviour
    {
        public Action<EnemyController> OnHit;
        [SerializeField] Transform _followTarget;
        [SerializeField] Collider _col;
        [SerializeField] HexHitterHammer _hitter;
        [SerializeField] float _speed;
        [SerializeField] float _rotateSpeed;
        [SerializeField] float _rotateDuration;
        Vector3 _offset;
        float _rotateLerpValue = 0;
        float _lastRotateTime = 0;
        bool _isRotate;

        void Start()
        {
            EnemyDict.RegistData(_col, this);
            Vector2 rand = UnityEngine.Random.insideUnitCircle;
            _offset.x = rand.x;
            _offset.z = rand.y;
        }

        private void Update()
        {
            transform.position += transform.forward * Time.deltaTime * _speed;

            if (_lastRotateTime + _rotateDuration > Time.deltaTime)
                StartRotate();

            if (!_isRotate)
                return;

            _rotateLerpValue = Mathf.Min(_rotateLerpValue + Time.deltaTime * _rotateSpeed, 1f);
            transform.forward = Vector3.Lerp(transform.forward, (_followTarget.position + _offset - transform.position).normalized, _rotateLerpValue);

            if (_rotateLerpValue == 1f)
            {
                _isRotate = false;
            }
        }

        private void StartRotate()
        {
            _rotateLerpValue = 0;
            _isRotate = true;
        }

        public void GetDamage()
        {
            OnHit?.Invoke(this);
        }
    }
}