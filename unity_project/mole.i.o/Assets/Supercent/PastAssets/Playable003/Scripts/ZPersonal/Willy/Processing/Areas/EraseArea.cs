using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class EraseArea : MonoBehaviour
    {
        private static Stacker _playerInventory = null;
        public event Action OnInput;

        [SerializeField] TransitionTween _jumpMotion;
        [SerializeField] Transform _trashPoint;
        [SerializeField] protected float _delay = 0.2f;
        [SerializeField] EStackableItemType _itemType = 0;
        float _lastGivenTime = 0f;
        public bool _canUpdate = false;

        void OnTriggerStay(Collider other)
        {
            if (!_canUpdate) return;
            if (_delay + _lastGivenTime >= Time.time) return;

            if (_playerInventory == null)
            {
                _playerInventory = StackerLib.Get(other.gameObject);
            }

            GetItem();
        }

        void GetItem()
        {
            StackableItem currentItem;
            if (!_playerInventory.TryFilterdGiveItem((int)_itemType, out currentItem))
                return;

            _lastGivenTime = Time.time;
            if (currentItem == null)
                return;

            _jumpMotion.StartTransition(currentItem.transform, currentItem.transform, _trashPoint, currentItem.ReturnSelf);
            OnInput?.Invoke();
        }

        public void SetOnInput(Action action, bool _isRegist)
        {
            if (_isRegist)
                OnInput += action;
            else
                OnInput -= action;
        }

    }
}