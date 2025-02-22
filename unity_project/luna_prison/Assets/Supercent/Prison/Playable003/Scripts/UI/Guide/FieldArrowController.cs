using System;
using System.Collections;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class FieldArrowController : IStartable
    {
        public event Action OnShow;
        [SerializeField] TransitionTween _idleMotion;
        [SerializeField] TransitionTween _offMotion;
        [SerializeField] TransitionTween _onMotion;

        [Space]
        [SerializeField] Transform _arrowParent;
        [SerializeField] GameObject _arrow;
        [SerializeField] Transform _startPos;
        [SerializeField] Transform _pointPos;
        [SerializeField] Transform[] _target;
        [SerializeField] Transform _curTransform;
        Transform _originParent;

        public void StartSetup()
        {
            if (_curTransform != null)
                _arrowParent.position = _curTransform.position;

            _originParent = _arrowParent.parent;
            _idleMotion.StartTransition(_arrow.transform, _startPos, _pointPos);
        }

        public void StartChange(int index)
        {
            _curTransform = _target[index];
            _offMotion.StartTransition(_arrow.transform, _arrow.transform, Show);
        }
        public void Show(int index)
        {
            _curTransform = _target[index];
            Show();
        }

        public void Show()
        {
            _arrowParent.position = _curTransform.position;
            _arrow.transform.localPosition = _startPos.localPosition;
            _onMotion.StartTransition(_arrow.transform, _arrow.transform);
            OnShow?.Invoke();
        }

        public void Hide()
        {
            _offMotion.StartTransition(_arrow.transform, _arrow.transform);
        }

        public void SetParent(Transform parent)
        {
            if (parent == null)
            {
                _arrowParent.SetParent(_originParent);
                return;
            }
            _arrowParent.SetParent(parent);
        }
    }
}
