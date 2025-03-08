using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class TimerUI : MonoBehaviour
    {
        public event Action OnEnd;
        const int SEC_PER_MINUITE = 60;
        [SerializeField] TMP_Text _minuiteText;
        [SerializeField] TMP_Text _secText;
        [SerializeField] int _totalSeconds;
        StringBuilder _stringBuilder = new StringBuilder();
        float _lastUpdateTime;
        int _curMinuite;
        int _curSecond;
        bool _isUpdate = false;


        private void Start()
        {
            ResetTimer();
        }

        public void ResetTimer()
        {
            _curMinuite = _totalSeconds / SEC_PER_MINUITE;
            _curSecond = _totalSeconds - _curMinuite * SEC_PER_MINUITE;
            UpdateText();
        }

        public void StartTimer()
        {
            _totalSeconds = (int)GameManager.GetDynamicData(GameManager.EDynamicType.PlayTime);
            _isUpdate = true;
            _lastUpdateTime = Time.time;
        }

        private void Update()
        {
            if (!_isUpdate)
                return;

            if (_lastUpdateTime + 1 > Time.time)
                return;

            _lastUpdateTime += 1;
            _curSecond--;
            if (_curSecond < 0)
            {
                if (_curMinuite <= 0)
                {
                    OnEnd?.Invoke();
                    _isUpdate = false;
                    _curSecond = 0;
                }
                else
                {
                    _curMinuite--;
                    _curSecond = SEC_PER_MINUITE - 1;
                }
            }

            UpdateText();
        }

        private void UpdateText()
        {
            _stringBuilder.Clear();
            _minuiteText.text = _stringBuilder.Append(_curMinuite).ToString();
            _stringBuilder.Clear();
            _secText.text = _stringBuilder.Append(_curSecond).ToString();
        }
    }
}