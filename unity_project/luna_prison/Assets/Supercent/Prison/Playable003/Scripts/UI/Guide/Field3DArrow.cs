using UnityEngine;

namespace Supercent.DinoUniverse.Playable005
{
    public class Field3DArrow : MonoBehaviour
    {
        [Header("Default")]
        [SerializeField] GameObject _arrow;

        [Space(15f)]
        [Header("Animation")]
        [SerializeField] AnimationCurve _ease;
        [SerializeField] AnimationCurve _spinEase;
        [SerializeField] float _animateTime = 0.85f;
        [SerializeField] float _pokeHeight = 2f;
        [SerializeField] int _spinTimingCount = 4;

        Vector3 _currentPosition = Vector3.zero;
        Vector3 _startPos = Vector3.zero;
        Vector3 _endPos = Vector3.zero;
        Vector3 _gap = Vector3.zero;
        Vector3 _endRot = Vector3.zero;

        float _timer = 0;
        float _lerpValue = 0f;
        int _count = 0;

        private void Start()
        {
            if (_currentPosition == Vector3.zero)
                _currentPosition = transform.position;
            _timer = 0f;
            _lerpValue = 0f;
            _startPos = _arrow.transform.localPosition;
            _endPos = _startPos;
            _endPos.y -= _pokeHeight;
        }
        private void Update()
        {
            _timer += Time.deltaTime;
            _lerpValue = _timer / _animateTime;
            _arrow.transform.localPosition = Vector3.Lerp(_startPos, _endPos, _ease.Evaluate(_lerpValue));
            if (_count == _spinTimingCount - 1)
            {
                _endRot.y = Mathf.Lerp(0f, -360f, _spinEase.Evaluate(_lerpValue));
                _arrow.transform.localRotation = Quaternion.Euler(_endRot);
            }
            if (_lerpValue >= 1f)
            {
                _timer = 0f;
                _count = (_count + 1) % _spinTimingCount;
            }
        }
    }

}

