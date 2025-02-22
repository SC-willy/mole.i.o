using Luna.Unity;
using Supercent.PrisonLife.Playable001;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class MainCanvasMediator : InitManagedBehaviorBase
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] ScreenInputController _screenInputHandler;
        [SerializeField] MoneyUI _moneyUI = new MoneyUI();
        [SerializeField] GameObject _elevatorUI;
        protected override void _Init()
        {
            _screenInputHandler.SetCanvas(_canvas);
            ScreenInputController.OnDragEvent += ShowFirstMove;
            _moneyUI.Init();
        }
        private void ShowFirstMove()
        {
            ScreenInputController.OnDragEvent -= ShowFirstMove;
            Analytics.LogEvent("First Move", 0);
        }

        public void SetActiveElevatorUI(bool active)
        {
            _elevatorUI.SetActive(active);
        }

        protected override void _Release()
        {
            ScreenInputController.OnDragEvent -= ShowFirstMove;
            _moneyUI.Release();
        }

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            _canvas = GetComponentInChildren<Canvas>(true);
            _screenInputHandler = GetComponentInChildren<ScreenInputController>(true);
        }
#endif // UNITY_EDITOR
    }
}