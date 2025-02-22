using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class PassTimerArea : PassArea
    {
        [SerializeField] float _delay = 0.1f;
        float _reuseTime = 0f;
        bool _checkDelay = false;
        bool _isCheckedTime = false;

        private void Awake()
        {
            _canUse = true;
        }

        public override void WaitAndReuseArea()
        {
            _checkDelay = true;
            _reuseTime = Time.time;
        }

        protected override void Update()
        {
            if (_checkDelay)
            {
                if (Time.time - _reuseTime < _delay)
                    return;
                _checkDelay = false;
                _isCheckedTime = true;
            }

            else if (!_isCheckedTime)
                return;
            if (_isCameraMove)
                return;
            if (!_canUse)
                return;

            if (!_isEnter && !_isAutoMod)
                return;

            _isCheckedTime = false;

            _onFunction?.Invoke();
        }

    }
}