
using System.Collections;
using System.Text;
using Supercent.MoleIO.Playable001;
using Supercent.Util;
using TMPro;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class MainCanvasMediator : InitManagedBehaviorBase
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] ScreenInputController _screenInputHandler;
        [SerializeField] GameObject _timerEndCard;
        [SerializeField] GameObject _winUI;
        [SerializeField] GameObject _failUI;
        [SerializeField] TMP_Text _killCount;
        [SerializeField] float _winUiDelay = 5f;
        [SerializeField] float _failUiDelay = 1f;
        StringBuilder _stringBuilder = new StringBuilder();
        protected override void _Init()
        {
            _screenInputHandler.SetCanvas(_canvas);
        }
        protected override void _Release()
        {
        }

        public void GetKillCount()
        {

        }

        public void SetActiveTimerEndUI(bool isOn = true)
        {
            _timerEndCard.SetActive(isOn);
        }

        public void OpenWinUI()
        {
            StartCoroutine(CoOpenWinUi());
        }

        private IEnumerator CoOpenWinUi()
        {
            yield return CoroutineUtil.WaitForSeconds(_winUiDelay);
            _winUI.SetActive(true);
        }

        public void OpenFailUI()
        {
            StartCoroutine(CoOpenFailUi());
        }
        private IEnumerator CoOpenFailUi()
        {
            yield return CoroutineUtil.WaitForSeconds(_failUiDelay);
            _failUI.SetActive(true);
        }

        public void CountKill(int count)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(count);
            _killCount.text = _stringBuilder.ToString();
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