using System.Collections;
using System.Collections.Generic;
using System.Text;
using Supercent.MoleIO.InGame;
using TMPro;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    public class UpgradeController : MonoBehaviour
    {
        const int UPGRADE_COUNT = 3;
        const int MONEY_MIN_AMIOUNT = 1000;
        const string MONEY_TAIL = ".0K";
        const string LEVEL_HEADER = "Lv.";
        [SerializeField] GameObject _upgradeCanvas;
        [SerializeField] TMP_Text[] _levelText;
        [SerializeField] TMP_Text[] _costText;
        StringBuilder _stringBuilder = new StringBuilder();

        [Space]
        [SerializeField] Animation _moneyLessAnim;

        void Start()
        {
            UpdateCanvas();
        }

        private void UpdateCanvas()
        {
            _stringBuilder.Clear();
            _levelText[0].text = _stringBuilder.Append(LEVEL_HEADER).Append(PlayerData.SkillLevel1).ToString();

            _stringBuilder.Clear();
            _levelText[1].text = _stringBuilder.Append(LEVEL_HEADER).Append(PlayerData.SkillLevel2).ToString();

            _stringBuilder.Clear();
            _levelText[2].text = _stringBuilder.Append(LEVEL_HEADER).Append(PlayerData.SkillLevel3).ToString();

            int costRise = PlayerData.CostRiseValue;

            _stringBuilder.Clear();
            _costText[0].text = _stringBuilder.Append(PlayerData.SkillLevel1 * costRise / MONEY_MIN_AMIOUNT).Append(MONEY_TAIL).ToString();

            _stringBuilder.Clear();
            _costText[1].text = _stringBuilder.Append(PlayerData.SkillLevel2 * costRise / MONEY_MIN_AMIOUNT).Append(MONEY_TAIL).ToString();

            _stringBuilder.Clear();
            _costText[2].text = _stringBuilder.Append(PlayerData.SkillLevel3 * costRise / MONEY_MIN_AMIOUNT).Append(MONEY_TAIL).ToString();
        }
        private void OnTriggerEnter(Collider other)
        {
            _upgradeCanvas.SetActive(true);
            ScreenInputController.CutJoystickMove();
        }

        public void ExitUi()
        {
            _upgradeCanvas.SetActive(false);
        }

        public void Upgrade(int code)
        {
            if (code <= 0)
                code = 1;
            else if (code > UPGRADE_COUNT)
                code = 3;

            bool _isCanUpgrade = false;
            switch (code)
            {
                case 1:
                    _isCanUpgrade = PlayerData.TryUpgrade1();
                    break;
                case 2:
                    _isCanUpgrade = PlayerData.TryUpgrade2();
                    break;
                case 3:
                    _isCanUpgrade = PlayerData.TryUpgrade3();
                    break;
            }

            if (_isCanUpgrade)
                UpdateCanvas();
            else if (_moneyLessAnim != null)
                _moneyLessAnim.Play();

        }
    }
}