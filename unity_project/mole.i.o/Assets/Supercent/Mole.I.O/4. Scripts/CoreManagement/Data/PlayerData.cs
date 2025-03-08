using System;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    public static class PlayerData
    {
        public static event Action<int> OnChangeMoney;
        const string STAGE = "CurrentStage";
        const string SKILL_LV_1 = "SKLV1";
        const string SKILL_LV_2 = "SKLV2";
        const string SKILL_LV_3 = "SKLV3";
        const string MOENY = "PlayerBaseMoney";
        const string NAME = "Player";
        const string IS_VOLUME_ON = "IsMute";
        const string IS_HAPTIC = "IsHapticMute";
        public static int Stage { get; private set; }
        public static int SkillLevel1 { get; private set; }
        public static int SkillLevel2 { get; private set; }
        public static int SkillLevel3 { get; private set; }
        public static int Money { get; private set; }
        public static string Name { get; private set; }
        public static int IsVolumeOn { get; private set; }
        public static int IsHaptic { get; private set; }

        public static int CostRiseValue => (int)GameManager.GetDynamicData(GameManager.EDynamicType.CostPerUpgrade);

        public static void LoadData()
        {
            Stage = PlayerPrefs.GetInt(STAGE, 0);
            SkillLevel1 = PlayerPrefs.GetInt(SKILL_LV_1, 1);
            SkillLevel2 = PlayerPrefs.GetInt(SKILL_LV_2, 1);
            SkillLevel3 = PlayerPrefs.GetInt(SKILL_LV_3, 1);
            Money = PlayerPrefs.GetInt(MOENY, 0);
            Name = PlayerPrefs.GetString(NAME, "Player");
            IsVolumeOn = PlayerPrefs.GetInt(IS_VOLUME_ON, 1);
            IsHaptic = PlayerPrefs.GetInt(IS_HAPTIC, 1); ;
        }

        public static void SaveData()
        {
            PlayerPrefs.SetInt(STAGE, Stage);
            PlayerPrefs.SetInt(SKILL_LV_1, SkillLevel1);
            PlayerPrefs.SetInt(SKILL_LV_2, SkillLevel2);
            PlayerPrefs.SetInt(SKILL_LV_3, SkillLevel3);
            PlayerPrefs.SetInt(MOENY, Money);
            PlayerPrefs.SetInt(IS_VOLUME_ON, IsVolumeOn);
            PlayerPrefs.SetInt(IS_HAPTIC, IsHaptic); ;

        }

        public static void ClearData()
        {
            PlayerPrefs.DeleteKey(SKILL_LV_1);
            PlayerPrefs.DeleteKey(SKILL_LV_2);
            PlayerPrefs.DeleteKey(SKILL_LV_3);
            PlayerPrefs.DeleteKey(STAGE);
            PlayerPrefs.DeleteKey(MOENY);
            PlayerPrefs.DeleteKey(NAME);
            PlayerPrefs.DeleteKey(IS_VOLUME_ON);
            PlayerPrefs.DeleteKey(IS_HAPTIC); ;
        }

        public static void SetName(string name)
        {
            Name = name;
            PlayerPrefs.SetString(NAME, Name);
        }
        public static bool TryUpgrade1()
        {
            if (!CheckMoneyByLevel(SkillLevel1))
                return false;

            SkillLevel1++;
            PlayerPrefs.SetInt(SKILL_LV_1, SkillLevel1);
            return true;
        }
        public static bool TryUpgrade2()
        {
            if (!CheckMoneyByLevel(SkillLevel2))
                return false;

            SkillLevel2++;
            PlayerPrefs.SetInt(SKILL_LV_2, SkillLevel2);
            return true;
        }
        public static bool TryUpgrade3()
        {
            if (!CheckMoneyByLevel(SkillLevel3))
                return false;

            SkillLevel3++;
            PlayerPrefs.SetInt(SKILL_LV_3, SkillLevel3);
            return true;
        }

        private static bool CheckMoneyByLevel(int level)
        {
            if (Money < level * CostRiseValue)
                return false;

            Money -= level * CostRiseValue;
            OnChangeMoney?.Invoke(Money);
            PlayerPrefs.SetInt(MOENY, Money);
            return true;
        }

        public static void EarnMoney(int money)
        {
            Money += money;
            OnChangeMoney?.Invoke(Money);
            PlayerPrefs.SetInt(MOENY, Money);
        }

        public static void SetActiveAudio(bool isOn)
        {
            IsVolumeOn = isOn ? 1 : 0;
            PlayerPrefs.SetInt(IS_VOLUME_ON, IsVolumeOn);
        }
        public static void SetActiveHaptic(bool isOn)
        {
            IsHaptic = isOn ? 1 : 0;
            PlayerPrefs.SetInt(IS_HAPTIC, IsHaptic);
        }
    }
}