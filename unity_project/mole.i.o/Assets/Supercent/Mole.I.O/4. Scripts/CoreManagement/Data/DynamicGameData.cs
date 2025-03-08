using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    [CreateAssetMenu(fileName = "DynamicGameData", menuName = "Data/DynamicGameData")]
    public class DynamicGameData : ScriptableObject
    {
        public List<LoadedData> LoadedDatas;

        public Dictionary<int, float> GetDataDic()
        {
            Dictionary<int, float> dataDic = new Dictionary<int, float>();
            foreach (var text in LoadedDatas)
            {
                if (!dataDic.ContainsKey(text.Key))
                    dataDic.Add(text.Key, text.Value);
            }
            return dataDic;
        }
    }

    [System.Serializable]
    public class LoadedData
    {
        public int Key;
        public float Value;
    }
}