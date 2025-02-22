using System;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class Stacker
    {
        // Public Data  ---------------------------------------
        public event Action OnStackMotionEnd;

        public void ChangeStackPos(Vector3 pos) => _stackTransform.position = pos;
        public InventoryData Data => _data;
        public Transform CurrentStackTransform => _stackTransform;
        public Vector3 CurrentSpawnPos => _stackTransform.position + _stackedHeight;
        public int Count => _data.Count;
        public bool IsCanGet => _data.IsCanGet;
        public bool IsHideMode => _isHideMode;

        // Others  ---------------------------------------
        [Space(10f)]
        [SerializeField] InventoryData _data;
        [SerializeField] TransitionJump _getMotion;
        [SerializeField] GameObject _maxText;
        [SerializeField] Transform _stackTransform;

        protected List<StackableItem> _items = new List<StackableItem>();
        protected StackableItem _currentGetItemInfo;
        protected StackableItem _currentGiveItemInfo;
        Vector3 _stackedHeight = Vector3.zero;


        [Space(10f)]
        [Header("Options")]
        [SerializeField] bool _isHideMode = false;
        [SerializeField] bool _isLimitViewMode = false;
        [SerializeField] int _limitViewCount = 12;
        bool _isLimitViewItemAdd { get { return _isLimitViewMode && _limitViewCount <= _data.Count - 1; } }
        bool _isLimitViewItemRelease { get { return _isLimitViewMode && _limitViewCount <= _data.Count; } }

        Stacker()
        {
            _data = new InventoryData();
            _data.OnMax += HandleOnMax;
            _data.OnReleaseMax += HandleOnReleaseMax;
        }

        public bool TryGetItem(StackableItem itemObject)
        {
            if (!_data.TryGetItem())
                return false;

            _currentGetItemInfo = itemObject;

            GetItem();

            _items.Add(_currentGetItemInfo);
            return true;
        }

        protected virtual void GetItem()
        {
            if (_isHideMode)
            {
                _stackedHeight.y = 0;
            }

            _currentGetItemInfo.transform.SetParent(_stackTransform);
            if (_getMotion != null)
            {
                StackableItem item = _currentGetItemInfo;
                item.SetActiveTrue();
                if (_isHideMode || _isLimitViewItemAdd)
                {
                    _stackedHeight.y -= _currentGetItemInfo.StackHeight;
                    _getMotion.StartTransition(_currentGetItemInfo.transform, _stackTransform, _stackedHeight, OnStackMotionEnd + item.SetActiveFalse);
                }
                else
                {
                    _getMotion.StartTransition(_currentGetItemInfo.transform, _stackTransform, _stackedHeight, OnStackMotionEnd);
                }
            }

            _stackedHeight.y += _currentGetItemInfo.StackHeight;
        }

        public bool TryGiveItem(out StackableItem item)
        {
            if (!_data.TryGiveItem())
            {
                item = null;
                return false;
            }

            _currentGiveItemInfo = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);

            GiveItem();

            _currentGiveItemInfo.StopAllCoroutines();

            item = _currentGiveItemInfo;
            return true;
        }

        protected virtual void GiveItem()
        {
            if (!_isLimitViewItemRelease)
            {
                _stackedHeight.y -= _currentGiveItemInfo.StackHeight;
            }
            if (_isHideMode)
            {
                _stackedHeight.y = 0;
            }
        }
        public bool TryFilterdGiveItem(int Type, out StackableItem item)
        {
            if (_data.IsEmpty)
            {
                item = null;
                return false;
            }

            int findedIndex = _items.Count - 1;
            if (Type >= 0)
            {

                bool isFind = false;

                for (int i = _items.Count - 1; i >= 0; i--)
                {
                    if (_items[i].TypeID == Type)
                    {
                        isFind = true;
                        break;
                    }
                    findedIndex--;
                }

                if (!isFind)
                {
                    item = null;
                    return false;
                }
            }


            _data.TryGiveItem();

            _currentGiveItemInfo = _items[findedIndex];
            _items.RemoveAt(findedIndex);

            GiveItem();
            ResetReleasedItemHeight(findedIndex);

            _currentGiveItemInfo.StopAllCoroutines();

            item = _currentGiveItemInfo;
            return true;
        }

        protected virtual void ResetReleasedItemHeight(int startedIndex)
        {
            if (startedIndex >= _items.Count)
                return;

            float stackHeight = _currentGiveItemInfo.StackHeight;
            Vector3 lowerPos;

            for (int i = startedIndex; i < _items.Count; i++)
            {
                lowerPos = _items[i].transform.position;
                lowerPos.y -= stackHeight;
                _items[i].transform.position = lowerPos;
            }
        }

        public void ActiveHideMode(bool isHide = true)
        {
            _isHideMode = isHide;

            if (_isHideMode)
            {
                for (int i = 0; i < _data.Count; i++)
                {
                    _items[i].transform.localScale = Vector3.zero;
                }
                _stackedHeight.y = 0;
            }
            else
            {
                _stackedHeight.y = 0;
                for (int i = 0; i < _data.Count; i++)
                {
                    _items[i].transform.localScale = Vector3.one;
                    _items[i].transform.position += _stackTransform.position + _stackedHeight;
                    _stackedHeight.y += _items[i].StackHeight;
                }
            }
        }

        public void ReleaseInventory()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].ReturnSelf();
            }
            _stackedHeight = Vector3.zero;
            _items.Clear();
            _data.Clear();
        }

        // Detaild Events  ---------------------------------------

        private void HandleOnMax()
        {
            if (_maxText == null)
                return;
            _maxText.SetActive(true);
        }

        private void HandleOnReleaseMax()
        {
            if (_maxText == null)
                return;
            _maxText.SetActive(false);
        }

        public void SetOnStack(Action action, bool _isRegist)
        {
            if (_isRegist)
                Data.OnStack += action;
            else
                Data.OnStack -= action;
        }
    }
}
