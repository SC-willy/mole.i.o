using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{

    [Serializable]
    public class GuideManager : IStartable
    {
        const int INDEX_POINT_2F = 2;
        // const int OVERLAY = 8;
        [SerializeField] FieldArrowController _fieldArrow;
        [SerializeField] ArrowLoop _playerArrow;
        [SerializeField] ArrowLoop _arrow1F;
        [SerializeField] ArrowLoop _arrow2f;
        [SerializeField] ArrowLoop _arrow2fFake;
        //[SerializeField] ArrowLoop _elevatorArrow;
        [SerializeField] ArrowLoop _escalaotrArrow;
        [SerializeField] ArrowLoop _escalaotrArrowFake;
        [SerializeField] Transform _roomPointer;
        [SerializeField] Animation _fadeAnimation;
        [SerializeField] Animation _fadeAnimation2f;

        [Space]
        [Header("Elevator Variate")]
        [SerializeField] Transform _fakeArrow2fStartTr;
        [SerializeField] Transform _arrow1fEndTr;

        bool _isElevator = false;
        bool _is2fFlow = false;
        bool _isFirst = true;
        public void GetIsElevatorFlow()
        {
            _isElevator = true;
            _arrow2fFake.SetStartTr(_fakeArrow2fStartTr);
            _arrow1F.SetEndTr(_arrow1fEndTr);
        }

        public void StartSetup()
        {
            _fieldArrow.StartSetup();
            _playerArrow.SetEndTr();
            _fieldArrow.OnShow += SetEndTr;
        }

        private void SetEndTr()
        {
            _playerArrow.SetEndTr();
            _arrow2f.SetEndTr();

            if (_is2fFlow)
                return;
            _playerArrow.gameObject.SetActive(true);
            _fadeAnimation.Rewind();
            _fadeAnimation.Play();
        }

        public void Guide(int guidePos)
        {
            _fieldArrow.StartChange(guidePos);

            if (guidePos == INDEX_POINT_2F)
            {
                _playerArrow.gameObject.SetActive(false);
                Active2fArrow(true);
                _fadeAnimation2f.Rewind();
                _fadeAnimation2f.Play();
                return;
            }
            if (guidePos == 0)
            {
                if (_isFirst)
                    _isFirst = false;
            }

            if (!_is2fFlow)
                return;
            Active2fArrow(false);
        }

        public void Hide()
        {
            _fieldArrow.Hide();
            _playerArrow.gameObject.SetActive(false);
            Active2fArrow(false);
        }

        private void Active2fArrow(bool isOn)
        {
            _is2fFlow = isOn;
            _arrow1F.gameObject.SetActive(isOn);
            _arrow2f.gameObject.SetActive(false);
            _arrow2fFake.gameObject.SetActive(isOn);

            if (isOn)
                _fieldArrow.SetParent(_roomPointer);
            else
                _fieldArrow.SetParent(null);


            // if (_isElevator)
            //     _elevatorArrow.gameObject.SetActive(isOn);
            // else
            if (!_isElevator)
                _escalaotrArrowFake.gameObject.SetActive(isOn);

            _escalaotrArrow.gameObject.SetActive(false);
        }

        public void CheckEnter(CustomerMoveFollower.EFloorState floor)
        {
            if (!_is2fFlow)
                return;

            if (floor == CustomerMoveFollower.EFloorState.Floor2)
            {
                if (!_isElevator)
                {
                    _escalaotrArrow.gameObject.SetActive(true);
                    _escalaotrArrowFake.gameObject.SetActive(false);
                }
            }
            _arrow1F.gameObject.SetActive(false);
        }
        public void CheckEnter(int floor) => CheckEnter((CustomerMoveFollower.EFloorState)floor);

        public void CheckExit(CustomerMoveFollower.EFloorState floor)
        {
            if (!_is2fFlow)
                return;

            bool is2F = floor == CustomerMoveFollower.EFloorState.Floor2;

            _arrow1F.gameObject.SetActive(!is2F);
            _arrow2f.gameObject.SetActive(is2F);
            _arrow2fFake.gameObject.SetActive(!is2F);

            if (_isElevator)
                return;

            _escalaotrArrow.gameObject.SetActive(false);
            _escalaotrArrowFake.gameObject.SetActive(!is2F);
        }
        public void CheckExit(int floor) => CheckExit((CustomerMoveFollower.EFloorState)floor);

        public void OnChangeFloor(int floor)
        {
            if (!_is2fFlow)
                return;

            // if (!_isElevator)
            //     return;
            // _elevatorArrow.gameObject.SetActive(!(floor == (int)CustomerMoveFollower.EFloorState.Floor2));
        }

        public void ChangeArrowPointer(Vector3 pos)
        {
            _roomPointer.position = pos;
            _arrow2fFake.SetEndTr();
        }
    }
}