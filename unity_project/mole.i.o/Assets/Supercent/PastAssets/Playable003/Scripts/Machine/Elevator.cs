using System;
using System.Collections;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class Elevator : MultiWaypointsChecker
    {
        const float CLAMPED_MIN_POS_Z = -0.9f;
        const float PUSH_TIME = 0.3f;
        const float PUSH_SPEED = 4f;
        const float CLAMP_UP = 0.85f;
        const float CLAMP_DOWN = 0.15f;
        private static readonly int _animBoolOpen = Animator.StringToHash("Open");
        public event Action<Elevator> OnElevatorIn;
        public event Action OnElevatorExit;
        public event Action OnReady;
        public event Action<int> OnReadyOut;
        public event Action OnEndUp;
        public event Action OnEndDown;

        public CustomerMoveFollower.EFloorState Floor => _floor;
        public Transform Model => _model;

        [Header("Default")]
        [SerializeField] Transform _floorTr;
        [SerializeField] GameObject _entranceCollider1F;
        [SerializeField] GameObject _entranceCollider2F;
        [SerializeField] float _speed = 1f;

        [Space]
        [Header("Anim")]
        [SerializeField] TransitionTween _openMotion;
        [SerializeField] Transform _model;
        [SerializeField] Animator _animator;
        [SerializeField] Animator _bar1fAnim;
        [SerializeField] Animator _bar2fAnim;

        [Space]
        [Header("Sound")]
        [SerializeField] AudioSource _bell;
        [SerializeField] AudioSource _stuck;
        CustomerMoveFollower.EFloorState _floor = CustomerMoveFollower.EFloorState.Floor1;
        Transform _playerOriginParent;
        Transform _playerTr;
        Vector3 _pushPos;
        Coroutine _pushCoroutine;

        private float _animationProgress = 1;
        int _enteringCustomers = 0;

        protected void Awake()
        {
            _bar1fAnim.SetBool(_animBoolOpen, true);
        }
        void OnEnable()
        {
            if (_openMotion != null)
                _openMotion.StartTransition(_model, _model);
        }

        public override int GetLeftIndexForUse()
        {
            for (int i = 0; i < _enabled.Length; i++)
            {
                if (_enabled[i])
                {
                    _enabled[i] = false;
                    _enteringCustomers++;
                    return i;
                }

            }
            return -1;
        }

        public void AlertEnterCustomer()
        {
            _enteringCustomers--;
            if (_enteringCustomers <= 0)
            {
                OnReady?.Invoke();
                for (int i = 0; i < _enabled.Length; i++)
                {
                    _enabled[i] = true;
                }
            }
        }

        public void MoveElevatorUp()
        {
            _animationProgress = Mathf.Clamp01(_animationProgress -= Time.deltaTime * _speed);
            _animator.Play("Elevator_Down", 0, Mathf.Clamp01(_animationProgress));
            _entranceCollider1F.gameObject.SetActive(true);
            _bar1fAnim.SetBool(_animBoolOpen, false);
            if (_animationProgress <= CLAMP_DOWN)
            {
                _entranceCollider2F.gameObject.SetActive(false);
                _bar2fAnim.SetBool(_animBoolOpen, true);
                if (_floor == CustomerMoveFollower.EFloorState.Floor1)
                {
                    OnEndUp?.Invoke();
                    _floor = CustomerMoveFollower.EFloorState.Floor2;
                    OnReadyOut?.Invoke((int)_floor);
                    _bell.Play();
                }
            }


        }
        public void CheckPlayerClampPos()
        {
            if (_playerTr.localPosition.z >= CLAMPED_MIN_POS_Z)
                return;
            _pushPos = _playerTr.localPosition;
            _pushPos.z = CLAMPED_MIN_POS_Z;

            if (_pushCoroutine == null)
                _pushCoroutine = StartCoroutine(PushPlayer());
        }
        IEnumerator PushPlayer()
        {
            float pushEndTime = Time.time + PUSH_TIME;
            while (Time.time < pushEndTime)
            {
                _playerTr.localPosition = Vector3.Lerp(_playerTr.localPosition, _pushPos, Time.deltaTime * PUSH_SPEED);
                yield return null;
            }
            _pushCoroutine = null;
        }
        public void MoveElevatorDown()
        {
            _animationProgress = Mathf.Clamp01(_animationProgress += Time.deltaTime * _speed);
            _animator.Play("Elevator_Down", 0, Mathf.Clamp01(_animationProgress));
            _entranceCollider2F.gameObject.SetActive(true);
            _bar2fAnim.SetBool(_animBoolOpen, false);
            if (_animationProgress >= CLAMP_UP)
            {
                _entranceCollider1F.gameObject.SetActive(false);
                _bar1fAnim.SetBool(_animBoolOpen, true);
                if (_floor == CustomerMoveFollower.EFloorState.Floor2)
                {
                    OnEndDown?.Invoke();
                    _floor = CustomerMoveFollower.EFloorState.Floor1;
                    OnReadyOut?.Invoke((int)_floor);
                    _bell.Play();
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            _animator.speed = 0;
            if (_playerOriginParent == null)
                _playerOriginParent = other.transform.parent;
            if (_playerTr == null)
                _playerTr = other.transform;

            other.transform.SetParent(_floorTr);
            OnElevatorIn?.Invoke(this);
        }

        void OnTriggerExit(Collider other)
        {
            OnElevatorExit?.Invoke();
            other.transform.SetParent(_playerOriginParent);
        }

        public void BindWithFloor(Transform target, bool isBind)
        {

            if (isBind)
            {
                target.transform.SetParent(_floorTr);
            }
            else
            {
                target.transform.SetParent(null);
            }
        }
    }
}