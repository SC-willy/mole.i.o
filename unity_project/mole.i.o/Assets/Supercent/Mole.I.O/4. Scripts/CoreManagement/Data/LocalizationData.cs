using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Data/Localization")]
    public class LocalizationData : ScriptableObject
    {
        public List<LocalizedText> Texts;

        public Dictionary<string, string> GetTextDic()
        {
            Dictionary<string, string> _stringDic = new Dictionary<string, string>();
            foreach (var text in Texts)
            {
                if (!_stringDic.ContainsKey(text.key))
                    _stringDic.Add(text.key, text.value);
            }
            return _stringDic;
        }
    }

    [System.Serializable]
    public class LocalizedText
    {
        public string key;
        public string value;
    }
}