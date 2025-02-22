using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Supercent.MoleIO.InGame
{
    public class ScreenInputController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
    {
        public static event Action OnPointerDownEvent;
        public static event Action OnDragEvent;
        public static event Action OnPointerUpEvent;

        private static event Action<bool> _onActiveJoystick;
        private static event Action _onCutJoystickMove;

        private static Vector2 _dir;
        public static Vector2 Direction => _dir;
        public static float X => _dir.x;
        public static float Y => _dir.y;

        public static void ActiveJoystick(bool on = true) => _onActiveJoystick?.Invoke(on);
        public static void CutJoystickMove() => _onCutJoystickMove?.Invoke();

        public static void StopJoystickHard()
        {
            CutJoystickMove();
            ActiveJoystick(false);
        }


        [SerializeField] Joystick _joystick;
        bool _isCutted = false;
        bool _isJoystickOff = false;

        public void SetCanvas(Canvas canvas)
        {
            _joystick.SetCanvas(canvas);
            _joystick.Init();
        }

        private void Start()
        {
            _onActiveJoystick += _ActiveJoystick;
            _onCutJoystickMove += _CutJoystickMove;
        }

        private void OnDestroy()
        {
            _onActiveJoystick -= _ActiveJoystick;
            _onCutJoystickMove -= _CutJoystickMove;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isCutted)
                return;

            if (_isJoystickOff)
                return;

            _joystick.OnDrag(eventData);
            OnDragEvent?.Invoke();
            _dir = _joystick.Direction;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isJoystickOff)
                return;

            _isCutted = false;
            _joystick.OnPointerDown(eventData);
            OnPointerDownEvent?.Invoke();
            _dir = Vector2.zero;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isJoystickOff)
                return;

            _isCutted = false;
            _joystick.OnPointerUp(eventData);
            OnPointerUpEvent?.Invoke();
            _dir = Vector2.zero;
        }

        private void _ActiveJoystick(bool on = true)
        {
            _joystick.SetActive(on);
            _isJoystickOff = !on;

            if (on)
            {
                OnPointerDownEvent?.Invoke();
            }
            else
            {
                OnPointerUpEvent?.Invoke();
            }

            _dir = _joystick.Direction;
        }

        private void _CutJoystickMove()
        {
            _isCutted = true;
            _joystick.FakeTurnOff();
            OnPointerUpEvent?.Invoke();
            _dir = Vector2.zero;
        }
    }
}
