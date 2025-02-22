using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class Escalator : MonoBehaviour
    {
        public event Action<int> OnEnter;
        public event Action OnExit;

        public CustomerMoveFollower.EFloorState Destination => _destination;

        [SerializeField] CustomerMoveFollower.EFloorState _destination;
        [SerializeField] TransitionTween _transition;
        [SerializeField] Transform _endPos;
        [SerializeField] ParticleSystem _elevatorPad;
        [SerializeField] float _animSpeed = 2f;
        private void Start()
        {
            if (_elevatorPad != null)
            {
                var curMain = _elevatorPad.main;
                curMain.simulationSpeed = _animSpeed;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            OnEnter?.Invoke((int)_destination);
            _transition.StartTransition(other.transform, _endPos, EndEscalator);
        }

        public void TransitionCustomer(Customer customer)
        {
            _transition.StartTransition(customer.transform, _endPos, customer.InvokeCustomerAction);
        }

        private void EndEscalator()
        {
            OnExit?.Invoke();
        }
    }
}