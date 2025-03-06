using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Data/Localization")]
    public class DynamicGameData : ScriptableObject
    {
        public enum EDynamicType
        {
            LevelPerUpgrade = 1001,
        }
        public List<LocalizedText> Texts;

        public Dictionary<int, float> GetDataDic()
        {
            Dictionary<int, float> dataDic = new Dictionary<int, float>();
            foreach (var text in Texts)
            {
                if (!dataDic.ContainsKey(text.Key))
                    dataDic.Add(text.Key, text.Value);
            }
            return dataDic;
        }
    }

    [System.Serializable]
    public class LocalizedText
    {
        public int Key;
        public float Value;
    }
}