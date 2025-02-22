using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    public static class PlayerData
    {
        const string STAGE = "CurrentStage";
        const string SKILL_LV_1 = "SKLV1";
        const string SKILL_LV_2 = "SKLV2";
        const string SKILL_LV_3 = "SKLV3";
        const string MOENY = "PlayerBaseMoney";
        const string NAME = "PlayerName";
        public static int Stage { get; private set; }
        public static int SkillLevel1 { get; private set; }
        public static int SkillLevel2 { get; private set; }
        public static int SkillLevel3 { get; private set; }
        public static int Money { get; private set; }
        public static string Name { get; private set; }

        public static void LoadData()
        {
            Stage = PlayerPrefs.GetInt(STAGE, 1);
            SkillLevel1 = PlayerPrefs.GetInt(SKILL_LV_1, 1);
            SkillLevel2 = PlayerPrefs.GetInt(SKILL_LV_2, 1);
            SkillLevel3 = PlayerPrefs.GetInt(SKILL_LV_3, 1);
            Money = PlayerPrefs.GetInt(MOENY, 1);
            Name = PlayerPrefs.GetString(NAME, "Player");
        }
    }
}