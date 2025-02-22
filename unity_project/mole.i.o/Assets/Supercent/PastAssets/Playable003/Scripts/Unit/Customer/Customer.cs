using System;
using System.Collections;
using Supercent.Util;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{

    public class Customer : PooledObject
    {
        private const float ANIM_TRANSITION_SPEED = 5f;
        private const float ANIM_IDLE_DELAY_MAX = 3f;
        private const int ANIM_IDLE_COUNT = 3;

        private static int _animCode = 0;

        public static readonly int _animFloatMove = Animator.StringToHash("IsMove");
        public static readonly int _animBoolBind = Animator.StringToHash("IsBind");
        public static readonly int _animIdle2 = Animator.StringToHash("idle02");
        public static readonly int _animIdle3 = Animator.StringToHash("idle03");

        public enum ECustomerState
        {
            None,
            Rotate,
            RotateMove,
            CustomAction,
        }

        // Action
        public event Action OnRelease;
        public event Action<Customer> OnEndMove;
        public event Action<Customer> OnEndRotate;
        public event Action<Customer> OnArriveNode;
        public event Action<Customer> OnCustomerAction;

        [Header("Transforms")]
        [SerializeField] CustomerMoveFollower.EFloorState _floor = CustomerMoveFollower.EFloorState.Floor1;
        public CustomerMoveFollower.EFloorState Floor => _floor;
        public void SetFloor(CustomerMoveFollower.EFloorState floor) => _floor = floor;
        [SerializeField] Transform _modelTransform;
        [SerializeField] Transform _stackTransform;
        public Transform StackTransform => _stackTransform;

        [SerializeField] float _moveSpeed = 6f;
        [SerializeField] float _standingRotateTime = 0.1f;
        [SerializeField] float _moveRotateTime = 0.5f;
        [SerializeField] AnimationCurve _standingRotateCurve;

        [Space(15f)]
        [Header("Misc")]
        [SerializeField] Animator _animator;
        [SerializeField] GameObject[] _originModels;
        [SerializeField] GameObject[] _additionalModel;
        [SerializeField] BubbleCanvas _bubbleCanvas;
        [SerializeField] GameObject _sadImoji;
        [SerializeField] TransitionTween _bubbleAppear;
        [SerializeField] Transform _miningBubble;
        [SerializeField] GameObject _bedding;
        public BubbleCanvas BubbleCanvas => _bubbleCanvas;

        ECustomerState _state = ECustomerState.None;

        Vector3[] _destinations;
        Vector3 _destination;
        Vector3 _startPos;
        Vector3 _direction;
        Vector3 _originForward;
        Vector3 _savedForward;

        float _animChangeValue = -1f;
        float _animFloat = 0;
        float _rotateLerpValue = 0;
        float _timer = 0f;
        float _moveValue = 0f;
        float _maxMoveTime = 0;

        int _destinationIndex = 0;

        protected bool _isStack = false;
        bool _isMove = false;
        bool _willMove = false;
        bool _isForcedPlayWalkAnim = false;
        public bool IsForcedPlayWalkAnim => _isForcedPlayWalkAnim;


        private void Update()
        {
            _animFloat = Mathf.Clamp01(_animFloat + (Time.deltaTime * _animChangeValue * ANIM_TRANSITION_SPEED));
            _animator.SetFloat(_animFloatMove, _animFloat);
            switch (_state)
            {
                case ECustomerState.Rotate:
                    Rotate();
                    break;
                case ECustomerState.RotateMove:
                    RotateMove();
                    break;
                default:
                    break;
            }
        }

        public void SetActiveItem(bool _isGet = true)
        {
            for (int i = 0; i < _originModels.Length; i++)
            {
                _originModels[i].SetActive(!_isGet);
            }

            for (int i = 0; i < _additionalModel.Length; i++)
            {
                _additionalModel[i].SetActive(_isGet);
            }

            if (_isGet)
            {
                _bubbleCanvas.Show();
                _bubbleAppear.StartTransition(_bubbleCanvas.transform, _bubbleCanvas.transform);
            }
        }

        private void SetMaxMoveTime()
        {
            _maxMoveTime = (_destination - _startPos).magnitude / _moveSpeed;
        }

        public void ChangeState(ECustomerState state)
        {
            _state = state;
            StartState(_state);
        }

        public void StartMove(Vector3[] destinations)
        {
            _destinations = destinations;
            _destination = destinations[0];
            _destinationIndex = 0;

            ChangeState(ECustomerState.RotateMove);
        }
        public void StartMove(Vector3 destination)
        {
            _destinations = new Vector3[] { destination };
            _destination = destination;
            _destinationIndex = 0;

            ChangeState(ECustomerState.RotateMove);
        }

        public void StartRotate(Vector3 destination)
        {
            _destination = destination;
            _direction = (_destination - transform.position).normalized;
            _willMove = false;

            ChangeState(ECustomerState.Rotate);
        }

        public void StartRotateWithDirection(Vector3 destination)
        {
            _destination = transform.position + destination;
            _direction = destination.normalized;
            _willMove = false;

            ChangeState(ECustomerState.Rotate);
        }

        public void StartRotateWithDirection()
        {
            _destination = transform.position + _savedForward;
            _direction = _savedForward.normalized;
            _willMove = false;

            ChangeState(ECustomerState.Rotate);
        }

        public void SaveForwardInfo(Vector3 savedForward)
        {
            _savedForward = savedForward;
        }

        public void ChangeMoveAnim(bool _isMove)
        {
            _animChangeValue = _isMove ? 1 : -1;
            if (_isForcedPlayWalkAnim && !_isMove)
            {
                OnEndMove?.Invoke(this);
            }
            _isForcedPlayWalkAnim = _isMove;
        }

        public void SetRestMode()
        {
            BubbleCanvas.Hide();
            _sadImoji.SetActive(true);
        }

        private void StartState(ECustomerState state)
        {
            switch (state)
            {
                case ECustomerState.None:
                    _animChangeValue = -1;
                    break;
                case ECustomerState.Rotate:
                    _timer = 0f;
                    _rotateLerpValue = 0f;
                    _originForward = _modelTransform.forward;
                    break;
                case ECustomerState.RotateMove:
                    _animChangeValue = 1;
                    _isMove = true;
                    _timer = 0f;
                    _rotateLerpValue = 0f;
                    _originForward = _modelTransform.forward;
                    _startPos = transform.position;
                    SetMaxMoveTime();
                    break;
                case ECustomerState.CustomAction:
                    break;
                default:
                    break;
            }
        }
        void Rotate()
        {
            if (_rotateLerpValue >= 1f)
                return;

            _timer += Time.deltaTime;
            _rotateLerpValue = Mathf.Min(_timer / _standingRotateTime, 1f);
            _modelTransform.forward = Vector3.Lerp(_originForward, _direction, _standingRotateCurve.Evaluate(_rotateLerpValue));

            if (_rotateLerpValue < 1f)
                return;

            if (_willMove)
            {
                StartMove(_destinations);
            }
            else
            {
                ChangeState(ECustomerState.None);
                if (OnEndRotate == null)
                    return;
                OnEndRotate.Invoke(this);
            }
        }

        void RotateMove()
        {
            if (!_isMove)
                return;

            _timer += Time.deltaTime;
            _rotateLerpValue = Mathf.Min(_timer / _moveRotateTime, 1f);

            _direction = _destination - transform.position;

            _direction.y = 0;
            _modelTransform.forward = Vector3.Lerp(_originForward, _direction.normalized, _rotateLerpValue);

            _moveValue = Mathf.Min(_timer / _maxMoveTime, 1f);
            transform.position = Vector3.Lerp(_startPos, _destination, _moveValue);
            _isMove = _moveValue < 1f;

            if (_isMove)
                return;

            if (_destinationIndex + 1 < _destinations.Length)
            {
                _destinationIndex++;
                _startPos = _destination;
                _destination = _destinations[_destinationIndex];
                _isMove = true;
                _timer = 0;
                _originForward = _modelTransform.forward;

                SetMaxMoveTime();
                OnArriveNode?.Invoke(this);
            }
            else
            {
                ChangeState(ECustomerState.None);
                if (OnEndMove != null)
                {
                    OnEndMove.Invoke(this);
                }
            }
        }
        public virtual void ReleaseSelf()
        {
            _isStack = false;
            OnEndMove = null;
            OnCustomerAction = null;
            SetActiveItem(false);

            if (OnRelease != null)
            {
                OnRelease.Invoke();
            }
            ReturnSelf();
        }

        public void ReleaseData()
        {
            OnEndMove = null;
            OnEndRotate = null;
            OnCustomerAction = null;
            _animator.SetBool(_animBoolBind, false);
            _bedding.gameObject.SetActive(false);

            switch (_animCode)
            {
                case 0:
                    StartCoroutine(PlaySecondIdle(UnityEngine.Random.Range(0f, ANIM_IDLE_DELAY_MAX)));
                    break;
                case 1:
                    StartCoroutine(PlayThirdIdle(UnityEngine.Random.Range(0f, ANIM_IDLE_DELAY_MAX)));
                    break;
                default:
                    break;
            }

            _animCode = (_animCode + 1) % ANIM_IDLE_COUNT;
        }

        private IEnumerator PlaySecondIdle(float delay)
        {
            yield return CoroutineUtil.WaitForSeconds(delay);
            _animator.SetTrigger(_animIdle2);
        }

        private IEnumerator PlayThirdIdle(float delay)
        {
            yield return CoroutineUtil.WaitForSeconds(delay);
            _animator.SetTrigger(_animIdle3);
        }

        public void InvokeCustomerAction(Customer customer) => OnCustomerAction?.Invoke(customer);
        public void InvokeCustomerAction(Elevator elevator)
        {
            elevator.OnElevatorExit -= InvokeCustomerAction;
            OnCustomerAction?.Invoke(this);
        }

        public void InvokeCustomerAction() => OnCustomerAction?.Invoke(this);
        public void SetForward(Vector3 forward)
        {
            forward.y = 0;
            _modelTransform.forward = forward;
        }

        public void ShowMiningBubble()
        {
            _miningBubble.gameObject.SetActive(true);
            _bubbleAppear.StartTransition(_miningBubble, _miningBubble);
        }
    }
}