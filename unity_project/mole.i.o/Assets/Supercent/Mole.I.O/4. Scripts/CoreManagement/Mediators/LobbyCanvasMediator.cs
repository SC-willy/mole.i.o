
using System.Collections;
using System.Text;
using Supercent.MoleIO.Management;
using TMPro;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class LobbyCanvasMediator : InitManagedBehaviorBase
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] ScreenInputController _screenInputHandler;
        [SerializeField] TMP_Text _moneyText;
        StringBuilder _stringBuilder = new StringBuilder();
        protected override void _Init()
        {
            _screenInputHandler.SetCanvas(_canvas);
            UpdateMoneyUI(PlayerData.Money);
            PlayerData.OnChangeMoney += UpdateMoneyUI;
        }
        protected override void _Release()
        {
            PlayerData.OnChangeMoney -= UpdateMoneyUI;
        }

        private void UpdateMoneyUI(int money)
        {
            _stringBuilder.Clear();
            _moneyText.text = _stringBuilder.Append(money).ToString();
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