
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class CustomerManager : IStartable, IUpdateable, IBindable
    {
        [SerializeField] CustomerSpawner _spawner = new CustomerSpawner();

        [Header("Spawn")]
        [SerializeField] MonoBehaviour _coroutineOwner;
        [SerializeField] int _secondCustomerCount = 12;
        [SerializeField] bool _isSpawning = false;

        [Header("CustomerPath")]
        [SerializeField] CustomerPathHandler _registCounter;
        [SerializeField] Transform _registOutTr;
        [SerializeField] CustomerProgressLine _takeOutLine;
        [SerializeField] PassProcessArea _takeOutArea;
        [SerializeField] CustomerMoveFollower _playerFollower;
        [SerializeField] AudioSource _inputSound;

        [SerializeField] JailRoom[] _jails;
        [SerializeField] Elevator _elevator;
        [SerializeField] int _maxInputCount = 4;

        List<Customer> _endedCustomer = new List<Customer>();


        public void StartSetup()
        {
            _isSpawning = true;
            _spawner.OnSpawn += SendToCounter;
            _spawner.Init();

            for (int i = 0; i < _jails.Length; i++)
            {
                _jails[i].OnJailCheck += InputToJail;
            }

            _elevator.OnReadyOut += _playerFollower.SwitchFollowerFloor;
        }

        public bool CheckElevatorUse(Elevator elevator)
        {
            if (_playerFollower.IsNoCustomer)
                return false;

            _playerFollower.EnterOnElevator();
            for (int i = 0; i < _maxInputCount; i++)
            {
                if (_playerFollower.IsNoCustomer)
                    break;

                int pathIndex = elevator.GetLeftIndexForUse();
                if (pathIndex < 0)
                    break;

                Customer curCustomer = _playerFollower.GetElevatorCustomer();
                curCustomer.StartMove(elevator.GetRoute(pathIndex));
                curCustomer.OnEndMove += FinishTakeCustomers;
                curCustomer.OnCustomerAction += ReleaseCustomers;
                elevator.OnElevatorExit += curCustomer.InvokeCustomerAction;
            }
            return true;
        }

        public void FinishTakeCustomers(Customer customer)
        {
            customer.OnEndMove -= FinishTakeCustomers;
            _elevator.AlertEnterCustomer();
            _elevator.BindWithFloor(customer.transform, true);
        }

        private void ReleaseCustomers(Customer customer)
        {
            _elevator.BindWithFloor(customer.transform, false);
        }


        public void UpdateManualy(float dt)
        {
            if (_isSpawning)
                _spawner.UpdateManualy(dt);

            _playerFollower.UpdateManualy(dt);
        }

        private void SendToCounter(Customer customer)
        {
            _registCounter.RegistCustomer(customer);

            customer.OnCustomerAction += SendToWaitPoint;
        }
        private void SendToWaitPoint(Customer customer)
        {
            customer.OnCustomerAction -= SendToWaitPoint;

            customer.SetActiveItem(true);
            customer.StartMove(_registOutTr.position);

            customer.OnEndMove += FindWaitPos;
        }

        private void FindWaitPos(Customer customer)
        {
            customer.OnEndMove -= FindWaitPos;

            _takeOutLine.RegistCustomer(customer);

            customer.OnCustomerAction += FollowPlayer;
        }

        private void FollowPlayer(Customer customer)
        {
            customer.OnCustomerAction -= FollowPlayer;

            _playerFollower.AddFolowers(customer);

            if (_playerFollower.IsCanGet)
                return;

            _takeOutArea.StopCalculatable();
        }

        public void ChangeFloor(int floor)
        {
            _playerFollower.ChangeFloorOnEscalator((CustomerMoveFollower.EFloorState)floor);
        }

        public void EndChangeFloor()
        {
            _playerFollower.EndChangeFloor();
        }

        public void RegistCustomerFloorChange(Action<Customer> action, CustomerMoveFollower.EFloorState floor)
        {
            if (floor == CustomerMoveFollower.EFloorState.Floor1)
                _playerFollower.OnStartFloorMoveOne += action;
            else if (floor == CustomerMoveFollower.EFloorState.Floor2)
                _playerFollower.OnStartFloorMoveTwo += action;
        }

        private void InputToJail(JailRoom jail)
        {
            bool _isSoundPlayed = false;
            _takeOutArea.ActiveCalculatable();
            for (int i = 0; i < _maxInputCount; i++)
            {
                if (_playerFollower.IsNoCustomer)
                    return;

                int pathIndex = jail.GetLeftIndexForUse();
                if (pathIndex < 0)
                    return;

                if (!_isSoundPlayed)
                {
                    _isSoundPlayed = true;
                    _inputSound.Play();
                }
                Customer curCustomer = _playerFollower.GetCustomer();
                curCustomer.StartMove(jail.GetRoute(pathIndex));
                curCustomer.SetRestMode();
                _spawner.AlertCustomerRelease();
                curCustomer.OnEndMove += jail.CloseJail;
                curCustomer.OnEndMove += EndCustomerFlow;
            }
        }

        private void EndCustomerFlow(Customer customer)
        {
            customer.ReleaseData();
            customer.StartRotateWithDirection(Vector3.back);
            _endedCustomer.Add(customer);
        }

        public IEnumerator CoShowWork()
        {
            for (int i = 0; i < _endedCustomer.Count; i++)
            {
                _endedCustomer[i].ShowMiningBubble();
                yield return null;
            }
        }

        public void ReleaseNewCustomers() => _spawner.SetMaxCustomer(_secondCustomerCount);

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _coroutineOwner = mono;
            _jails = mono.GetComponentsInChildren<JailRoom>(true);
        }
#endif // UNITY_EDITOR

    }
}