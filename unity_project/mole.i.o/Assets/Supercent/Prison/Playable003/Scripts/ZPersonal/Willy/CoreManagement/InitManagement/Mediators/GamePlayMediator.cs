using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class GamePlayMediator : InitManagedBehaviorBase
    {
        public event Action<bool> OnInteractElevator;


        [Header("Variate")]
        [LunaPlaygroundField("Elevator Mod")]
        [SerializeField] bool _isElevator = false;

        [Space]
        [Header("Managers")]
        [CustomColor(0, 0, 0.2f)]
        [SerializeField] UnlockController _upgrade = new UnlockController();
        [CustomColor(0, 0, 0.3f)]
        [SerializeField] CustomerManager _customerManager = new CustomerManager();
        [CustomColor(0.1f, 0, 0.3f)]
        [SerializeField] GuideManager _guideManager = new GuideManager();
        [SerializeField] PlayerMediator _player;

        [Space]
        [Header("Escalator")]
        [SerializeField] GameObject _escalatorsObj;
        [SerializeField] TransitionTween _escalatorOpenMotion;
        [SerializeField] Escalator _upper;
        [SerializeField] Transform _upperTr;
        [SerializeField] Escalator _downer;
        [SerializeField] Transform _downerTr;
        [SerializeField] GameObject _escalatorIco;

        [Space]
        [Header("Elevator")]
        [SerializeField] Elevator _elevator;
        [SerializeField] GameObject _elevatorIco;
        [SerializeField] Transform _elevatorUnlockPad;
        [SerializeField] Transform _elevatorPadPos;

        [Space]
        [Header("Others")]
        [SerializeField] Transform _playerModelTr;
        [SerializeField] Transform _roomPointSecond;
        [SerializeField] Transform _roomPointThird;

        int _floor;
        protected override void _Init()
        {
            if (_isElevator)
            {
                _elevatorUnlockPad.position = _elevatorPadPos.position;
                _elevator.OnElevatorIn += OnEnterElevator;
                _elevator.OnReady += OnReadyElevator;
                _elevator.OnElevatorExit += OnElevatorExit;
                _elevator.OnReadyOut += _guideManager.OnChangeFloor;

                _escalatorsObj.gameObject.SetActive(false);
                _elevator.gameObject.SetActive(true);
                _escalatorIco.gameObject.SetActive(false);
                _elevatorIco.gameObject.SetActive(true);
                _guideManager.GetIsElevatorFlow();

                _elevator.Model.localScale = Vector3.zero;
            }
            else
            {
                _upper.OnEnter += OnEnterUpEscalator;
                _upper.OnExit += OnExitUpEscalator;
                _downer.OnEnter += OnEnterUpEscalator;
                _downer.OnExit += OnExitUpEscalator;
                _escalatorsObj.gameObject.SetActive(true);
                _elevator.gameObject.SetActive(false);
                _escalatorIco.gameObject.SetActive(true);
                _elevatorIco.gameObject.SetActive(false);
                _upperTr.localScale = Vector3.zero;
                _downerTr.localScale = Vector3.zero;
            }


            _customerManager.RegistCustomerFloorChange(_upper.TransitionCustomer, CustomerMoveFollower.EFloorState.Floor2);
            _customerManager.RegistCustomerFloorChange(_downer.TransitionCustomer, CustomerMoveFollower.EFloorState.Floor1);

            _guideManager.StartSetup();

            _upgrade.StartSetup();
        }
        private void OnEnterUpEscalator(int floor)
        {
            _floor = floor;
            ScreenInputController.ActiveJoystick(false);
            _player.ExecuteOnStop();
            _customerManager.ChangeFloor(floor);
            _guideManager.CheckEnter(floor);
        }

        private void OnExitUpEscalator()
        {
            ScreenInputController.ActiveJoystick(true);
            _customerManager.EndChangeFloor();
            _guideManager.CheckExit(_floor);
            _guideManager.OnChangeFloor(_floor);
        }

        private void OnEnterElevator(Elevator elevator)
        {
            _guideManager.CheckEnter(elevator.Floor);

            if (!_customerManager.CheckElevatorUse(elevator))
            {
                OnInteractElevator?.Invoke(true);
                return;
            }
            ScreenInputController.ActiveJoystick(false);
            _player.ExecuteOnStop();
        }

        private void OnReadyElevator()
        {
            ScreenInputController.ActiveJoystick(true);
            OnInteractElevator?.Invoke(true);
        }
        private void OnElevatorExit()
        {
            OnInteractElevator?.Invoke(false);
            _guideManager.CheckExit(_elevator.Floor);
        }
        protected override void _Release()
        {
        }

        private void Update()
        {
            _customerManager.UpdateManualy(Time.deltaTime);
        }

        public void StartSpawnPrisoner() => _customerManager.StartSetup();

        public void Guide(int index)
        {
            _guideManager.Guide(index);
        }

        public void Hide()
        {
            _guideManager.Hide();
        }

        public void ChangeRoomPointerSecond()
        {
            _guideManager.ChangeArrowPointer(_roomPointSecond.position);
        }
        public void ChangeRoomPointerThird()
        {
            _guideManager.ChangeArrowPointer(_roomPointThird.position);
        }

        public void ShowWork()
        {
            StartCoroutine(_customerManager.CoShowWork());
        }

        public void StopAllGuide() => _guideManager.Hide();

        public void ShoeEscalatorOpen()
        {
            if (_isElevator)
                return;
            if (_escalatorOpenMotion == null)
                return;

            _escalatorOpenMotion.StartTransition(_upperTr, _upperTr);
            _escalatorOpenMotion.StartTransition(_downerTr, _downerTr);
        }

        public void ReleaseNewCustomers() => _customerManager.ReleaseNewCustomers();

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            _upgrade.Bind(this);
            _customerManager.Bind(this);
            _player = GetComponentInChildren<PlayerMediator>();
        }
#endif // UNITY_EDITOR
    }
}