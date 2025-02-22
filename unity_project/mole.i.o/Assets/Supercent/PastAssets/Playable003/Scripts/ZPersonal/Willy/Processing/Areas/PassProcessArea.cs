using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class PassProcessArea : PassArea
    {
        BubbleCanvas _bubble;
        bool _isCalculatable = true;
        bool _isCalculating = false;

        public void SetBubble(BubbleCanvas bubble) => _bubble = bubble;
        protected override void Update()
        {
            if (_isCameraMove)
                return;
            if (!_canUse || !_isCalculatable)
                return;
            if (_bubble == null)
                return;

            if (_isEnter || _isAutoMod)
            {
                if (!_isCalculating)
                {
                    _bubble.StartFillGague(CompleteFill);
                    _isCalculating = true;
                }
            }
        }

        private void CompleteFill()
        {
            _bubble = null;
            _isCalculating = false;
            CompleteImmediately();
        }
        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            _isCalculating = false;

            if (_bubble != null)
                _bubble.StopFillGauge();
        }

        public void ActiveCalculatable()
        {
            _isCalculatable = true;
        }
        public void StopCalculatable()
        {
            _isCalculatable = false;
        }
    }
}