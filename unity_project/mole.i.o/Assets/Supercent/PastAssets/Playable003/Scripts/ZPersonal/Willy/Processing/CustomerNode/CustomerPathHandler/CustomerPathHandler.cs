using System;
using System.Collections.Generic;
using UnityEngine;


namespace Supercent.MoleIO.InGame
{
    public class CustomerPathHandler : InitManagedObject
    {
        public event Action OnCustomerArrive;
        protected Action _onCustomerLeave;
        public event Action OnCustomerLeave { add { _onCustomerLeave += value; } remove { _onCustomerLeave -= value; } }

        [SerializeField] private PathBase[] _paths;

        protected Queue<Customer> _customerLine = new Queue<Customer>();
        public int Count { get { return _customerLine.Count; } }
        [SerializeField] float _maxLineLength = 1;

        protected override void _Init()
        {
        }

        protected override void _Release()
        {
        }

        public void RegistCustomer(Customer customer)
        {
            if (_customerLine.Count <= 0)
            {
                customer.OnEndMove += ArriveToLine;
            }
            _customerLine.Enqueue(customer);
            customer.OnEndMove += RotateAfterStand;

            customer.StartMove(CalculateStandPosition(customer, _customerLine.Count - 1));
        }

        private Vector3 CalculateStandPosition(Customer customer, int index)
        {
            float standPosValue = index / _maxLineLength * _paths.Length;
            int lineIndex = (int)standPosValue;

            Vector3 standPos;

            if (lineIndex > _paths.Length - 1 && standPosValue - lineIndex == 0)
            {
                standPos = _paths[_paths.Length - 1].Evaluate(1);
                customer.SaveForwardInfo(_paths[_paths.Length - 1].GetForwardAngle());
                return standPos;
            }

            standPos = _paths[lineIndex].Evaluate(standPosValue - lineIndex);
            customer.SaveForwardInfo(_paths[lineIndex].GetForwardAngle());
            return standPos;
        }

        public void ArriveToLine(Customer customer)
        {
            customer.OnEndMove -= ArriveToLine;
            if (OnCustomerArrive == null)
                return;
            if (_customerLine.Count <= 0)
                return;
            OnCustomerArrive.Invoke();
        }

        public void CallNextCustomer()
        {
            _StartCallNextCustomer();

            if (_customerLine.Count <= 0)
                return;

            _EndCallNextCustomer();
        }

        protected virtual void _EndCallNextCustomer()
        {
            var customer = _customerLine.Dequeue();
            customer.InvokeCustomerAction(customer);

            _onCustomerLeave?.Invoke();

            if (_customerLine.Count <= 0)
                return;
            _customerLine.Peek().OnEndMove += ArriveToLine;
            ReSortLine();
        }

        protected virtual void _StartCallNextCustomer() { }

        public void ReSortLine()
        {
            int count = 0;
            foreach (var customer in _customerLine)
            {
                customer.StartMove(CalculateStandPosition(customer, count));
                customer.OnEndMove += RotateAfterStand;
                count++;
            }
        }

        public void RotateAfterStand(Customer customer)
        {
            customer.OnEndMove -= RotateAfterStand;
            customer.StartRotateWithDirection();
        }

        public void SetPath(PathBase pathBase)
        {
            _paths = new PathBase[] { pathBase };
        }

        public void SetOnCustomerLeave(Action action, bool _isRegist)
        {
            if (_isRegist)
                OnCustomerLeave += action;
            else
                OnCustomerLeave -= action;
        }
    }
}