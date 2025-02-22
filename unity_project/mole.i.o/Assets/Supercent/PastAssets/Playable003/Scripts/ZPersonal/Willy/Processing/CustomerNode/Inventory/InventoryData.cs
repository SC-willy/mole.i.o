using System;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class InventoryData
    {
        // Events   -------------------------------------
        protected Action _onFirstStack;
        public event Action OnFirstStack { add { _onFirstStack += value; } remove { _onFirstStack -= value; } }
        protected Action _onStack;
        public event Action OnStack { add { _onStack += value; } remove { _onStack -= value; } }
        protected Action _onMax;
        public event Action OnMax { add { _onMax += value; } remove { _onMax -= value; } }
        protected Action _onReleaseMax;
        public event Action OnReleaseMax { add { _onReleaseMax += value; } remove { _onReleaseMax -= value; } }
        protected Action _onRelease;
        public event Action OnRelease { add { _onRelease += value; } remove { _onRelease -= value; } }
        protected Action _onEmpty;
        public event Action OnEmpty { add { _onEmpty += value; } remove { _onEmpty -= value; } }


        // Public Data  ---------------------------------------
        public int Count => _count;
        public bool IsLimitable = true;
        public bool IsMax => IsLimitable && _count >= _maxCount;
        public bool IsEmpty => _count <= 0;
        public bool IsCanGet => !IsMax && _lastGetTime + _getDuration < Time.time;
        public void SetMaxCount(int count) => _maxCount = count;
        public void SetDuration(float duration) => _getDuration = duration;
        public void SetLimitable(bool isLimitable = true)
        {
            IsLimitable = isLimitable;
            if (!isLimitable)
                return;
            if (!IsMax)
                return;

            _onMax?.Invoke();
        }


        // Others  ---------------------------------------
        [SerializeField] int _maxCount = 12;
        [SerializeField] float _getDuration = 0.2f;
        float _lastGetTime = 0;
        int _count = 0;

        // Book System  ---------------------------------------
        // int _itemBookCount = 0;
        // public void Book() { _itemBookCount++; }
        // public void Book(int count) { _itemBookCount += count; }
        // public void UnBook() { _itemBookCount--; }
        // public void UnBook(int count) { _itemBookCount -= count; }
        // public int CountWithBook => _inventory.Count + _itemBookCount;
        //public bool IsMaxWithBook => IsLimitable && _inventory.Count + _itemBookCount >= _maxStack;

        public bool TryGetItem()
        {
            if (_lastGetTime + _getDuration > Time.time)
                return false;

            if (IsMax)
                return false;

            if (IsEmpty)
                _onFirstStack?.Invoke();

            _count++;
            _lastGetTime = Time.time;

            _onStack?.Invoke();

            if (IsLimitable && _count >= _maxCount)
                _onMax?.Invoke();
            return true;
        }

        public bool TryGiveItem()
        {
            if (IsEmpty)
            {
                return false;
            }

            if (IsMax)
                _onReleaseMax.Invoke();

            _count--;
            _onRelease?.Invoke();

            if (IsEmpty)
                _onEmpty?.Invoke();

            return true;
        }

        public void Clear() => _count = 0;

        public void SetOnPlayerStack(Action action, bool _isRegist)
        {
            if (_isRegist)
                OnStack += action;
            else
                OnStack -= action;
        }
    }
}