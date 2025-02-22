
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Supercent.PrisonLife.Playable003
{
    public class ElevatorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private static readonly int _animCode = Animator.StringToHash("Press");
        const float ANIM_FORWARD = 1f;
        const float ANIM_BACKWARD = -2f;
        [SerializeField] bool _isUp;
        [SerializeField] Elevator _elevator;
        [SerializeField] Animator _pressAnimation;
        [SerializeField] float _animSpeed = 0;
        float _animProgress = 0;
        float _animDir = ANIM_BACKWARD;
        bool _isHolding = false;
        void Update()
        {
            _animProgress = Mathf.Clamp01(_animProgress + (_animDir * Time.deltaTime * _animSpeed));
            _pressAnimation.Play(_animCode, 0, Mathf.Clamp01(_animProgress));

            if (!_isHolding)
                return;

            if (_isUp)
                _elevator.MoveElevatorUp();
            else
                _elevator.MoveElevatorDown();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isHolding = true;
            _animDir = ANIM_FORWARD;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHolding = false;
            _animDir = ANIM_BACKWARD;
        }

        public void SetElevator(Elevator elevator)
        {
        }
    }
}
