
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class ElevatorAutoButton : MonoBehaviour
    {
        private static readonly int _animPress = Animator.StringToHash("Press");
        private static readonly int _animGuide = Animator.StringToHash("Guide");
        [SerializeField] Elevator _elevator;
        [SerializeField] ElevatorTouchChecker _up;
        [SerializeField] ElevatorTouchChecker _down;
        [SerializeField] Animator _pressAnimationUp;
        [SerializeField] Animator _pressAnimationDown;
        [SerializeField] AudioSource _sound;
        bool _isUp;
        bool _isUpdate;

        void OnEnable()
        {
            if (_elevator.Floor == CustomerMoveFollower.EFloorState.Floor2)
            {
                _pressAnimationDown.Play(_animGuide);
            }
            else
            {
                _pressAnimationUp.Play(_animGuide);
            }
        }

        private void Start()
        {
            _elevator.OnElevatorExit += StopUpdate;
            _up.OnPointerDownEvent += MoveUp;
            _down.OnPointerDownEvent += MoveDown;
        }

        private void StopUpdate() => _isUpdate = false;
        private void Update()
        {
            if (!_isUpdate)
                return;

            if (_isUp)
                _elevator.MoveElevatorUp();
            else
                _elevator.MoveElevatorDown();
        }

        public void MoveUp()
        {
            _isUpdate = true;
            _isUp = true;
            _pressAnimationUp.Play(_animPress, 0, 0f);
            _elevator.CheckPlayerClampPos();
            _sound.Play();
        }

        public void MoveDown()
        {
            _isUpdate = true;
            _isUp = false;
            _pressAnimationDown.Play(_animPress, 0, 0f);
            _sound.Play();
        }
    }
}
