using System;
using UnityEngine;


namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class UnlockPadAnimator : IUpdateable
    {
        const string AnimAppear = "Appear";
        const string AnimDisAppear = "Disappear";

        public event Action OnUnlock;
        [HideInInspector] public bool IsEnter;

        [SerializeField] GameObject _lockCover;
        [SerializeField] GameObject _activeField;
        [SerializeField] Collider _colider;
        [SerializeField] Animator _animator;
        public void Unlock()
        {
            if (_lockCover != null)
                _lockCover.SetActive(false);
            if (_activeField != null)
                _activeField.SetActive(true);

            if (_animator != null)
            {
                _animator.enabled = true;
                _animator.Play(AnimAppear);
            }

            if (_colider != null)
                _colider.enabled = true;

            OnUnlock?.Invoke();
        }

        public void RemoveNode()
        {
            if (_animator == null)
                return;

            _animator.Play(AnimDisAppear);
        }

        public void UpdateManualy(float dt)
        {
            if (_animator == null)
                return;

            _animator.SetBool("IsEnter", IsEnter);
        }
    }
}