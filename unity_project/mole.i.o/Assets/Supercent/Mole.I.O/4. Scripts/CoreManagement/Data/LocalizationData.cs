using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Data/Localization")]
    public class LocalizationData : ScriptableObject
    {
        public List<LocalizedText> Texts;

        public Dictionary<int, string> GetTextDic()
        {
            Dictionary<int, string> _stringDic = new Dictionary<int, string>();
            foreach (var text in Texts)
            {
                if (!_stringDic.ContainsKey(text.Key))
                    _stringDic.Add(text.Key, text.Value);
            }
            return _stringDic;
        }
    }

    [System.Serializable]
    public class LocalizedText
    {
        public int Key;
        public string Value;
    }
}