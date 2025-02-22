using System;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class CustomerMoveFollower : IUpdateable
    {
        public enum EFloorState
        {
            None,
            Floor1,
            Floor2,
        }

        public event Action<Customer> OnStartFloorMoveTwo;
        public event Action<Customer> OnStartFloorMoveOne;

        public bool IsNoCustomer => _followers.Count <= 0;
        public bool IsCanGet => _curCount < _maxCount;
        [SerializeField] EFloorState _floorState = EFloorState.Floor1;
        [SerializeField] LayerMask _obstacleLayer;
        [SerializeField] MoveFollowerPosPad _followerPosPad;
        [SerializeField] Transform[] _followPoses;

        [SerializeField] Transform _floor1Entrance;
        [SerializeField] Transform _floor2Entrance;
        [SerializeField] Transform _floor1PadPos;
        [SerializeField] Transform _floor2PadPos;
        Vector3 _currentFixedPos;

        [SerializeField] float _moveSpeed = 2f;
        [SerializeField] float _obstacleAvoidRadius = 2f;
        [SerializeField] float _followPadding = 0.1f;

        [Header("Count")]
        [SerializeField] GameObject _maxText;
        [Range(1, 6)]
        [LunaPlaygroundField("Carry Count", 0, "Player")]
        [SerializeField] int _maxCount = 3;
        int _curCount = 0;

        List<Customer> _followers = new List<Customer>();
        List<Customer> _fixPosFollowers = new List<Customer>();

        public void AddFolowers(Customer customer)
        {
            if (!IsCanGet)
                return;

            _followers.Add(customer);
            _curCount++;

            if (IsCanGet)
                return;
            _maxText.SetActive(true);
        }

        public void UpdateManualy(float dt)
        {
            for (int i = 0; i < _followers.Count; i++)
            {
                Customer curFollower = _followers[i];

                if ((_followPoses[i].position - curFollower.transform.position).sqrMagnitude > _followPadding)
                {
                    MoveFollower(curFollower.transform, _followPoses[i].position);
                    curFollower.ChangeMoveAnim(true);
                    curFollower.SetForward((_followPoses[i].position - curFollower.transform.position).normalized);
                }
                else
                    curFollower.ChangeMoveAnim(false);
            }

            for (int i = 0; i < _fixPosFollowers.Count; i++)
            {
                Customer curFollower = _fixPosFollowers[i];
                if ((_currentFixedPos - curFollower.transform.position).sqrMagnitude > _followPadding)
                {
                    MoveFollower(curFollower.transform, _currentFixedPos);
                    curFollower.ChangeMoveAnim(true);
                    curFollower.SetForward((_currentFixedPos - curFollower.transform.position).normalized);
                }
                else
                    curFollower.ChangeMoveAnim(false);
            }
        }

        public void ChangeFloorOnEscalator(EFloorState floor)
        {
            _floorState = floor;
            _currentFixedPos = _floorState == EFloorState.Floor2 ? _floor1Entrance.position : _floor2Entrance.position;
            for (int i = 0; i < _fixPosFollowers.Count; i++)
            {
                Customer curCustomer = _fixPosFollowers[i];
                if (curCustomer.Floor == floor)
                {
                    _followers.Add(curCustomer);
                    _fixPosFollowers.RemoveAt(i);
                    i--;
                    curCustomer.OnEndMove -= StartWaitRenavigateEscalator;
                }
            }
            for (int i = 0; i < _followers.Count; i++)
            {
                Customer curCustomer = _followers[i];
                if (curCustomer.Floor != floor)
                {
                    curCustomer.SetFloor(floor);
                    _fixPosFollowers.Add(curCustomer);
                    _followers.RemoveAt(i);
                    i--;
                    curCustomer.OnEndMove += StartWaitRenavigateEscalator;
                }
            }
            _followerPosPad.SetMove(false);
            SwitchFollowerFloor(_floorState);
        }

        public void SwitchFollowerFloor(EFloorState floor)
        {
            _followerPosPad.transform.position = floor == EFloorState.Floor1 ? _floor1PadPos.position : _floor2PadPos.position;
        }

        public void SwitchFollowerFloor(int floor)
        {
            if (floor <= 1)
                SwitchFollowerFloor(EFloorState.Floor1);
            else
                SwitchFollowerFloor(EFloorState.Floor2);
        }

        public void EnterOnElevator()
        {
            for (int i = 0; i < _followers.Count; i++)
            {
                _followers[i].OnCustomerAction += ReAddFollowers;
            }
        }

        public Customer GetCustomer()
        {
            if (!IsCanGet)
                _maxText.SetActive(false);

            Customer _firstCustomer = _followers[0];
            _followers.RemoveAt(0);
            _curCount = Mathf.Max(_curCount - 1, 0);

            return _firstCustomer;
        }

        public Customer GetElevatorCustomer()
        {
            Customer _firstCustomer = _followers[0];
            _followers.RemoveAt(0);

            return _firstCustomer;
        }

        private void StartWaitRenavigateEscalator(Customer customer)
        {
            customer.OnEndMove -= StartWaitRenavigateEscalator;
            _fixPosFollowers.Remove(customer);
            customer.ChangeMoveAnim(false);

            if (_floorState == EFloorState.Floor2)
                OnStartFloorMoveTwo?.Invoke(customer);
            else
                OnStartFloorMoveOne?.Invoke(customer);

            customer.OnCustomerAction += ReAddFollowers;
        }

        public void EndChangeFloor()
        {
            _followerPosPad.SetMove(true);
        }

        private void ReAddFollowers(Customer customer)
        {
            customer.OnCustomerAction -= ReAddFollowers;
            _followers.Add(customer);
        }
        Vector3 AvoidObstacles(Vector3 targetPos, Transform follower)
        {
            RaycastHit hit;
            Vector3 moveDir = (targetPos - follower.position).normalized;

            // 벽이 감지되었는지 확인
            if (Physics.Raycast(follower.position, moveDir, out hit, _obstacleAvoidRadius, _obstacleLayer))
            {
                Vector3 bestDirection = Vector3.zero;
                float maxClearDistanceSqr = 0f;

                for (int i = 0; i < 3; i++)
                {
                    float angle = 90 + i * 40f;
                    Vector3 direction = Quaternion.Euler(0, angle, 0) * follower.forward;

                    if (!Physics.Raycast(follower.position, direction, _obstacleAvoidRadius, _obstacleLayer))
                    {
                        float clearDistanceSqr = (follower.position + direction * _obstacleAvoidRadius - targetPos).sqrMagnitude;

                        float directionSimilarity = Vector3.Dot(moveDir, direction);
                        float weightedDistance = clearDistanceSqr * (1 + directionSimilarity);

                        if (weightedDistance > maxClearDistanceSqr)
                        {
                            maxClearDistanceSqr = weightedDistance;
                            bestDirection = direction;
                        }
                    }
                }

                return bestDirection == Vector3.zero ? follower.forward : bestDirection;
            }
            else
            {
                return moveDir;
            }
        }
        void MoveFollower(Transform follower, Vector3 formationPos)
        {
            Vector3 obstacleAvoidance = AvoidObstacles(formationPos, follower);
            follower.position += obstacleAvoidance * _moveSpeed * Time.deltaTime;
        }
    }
}

