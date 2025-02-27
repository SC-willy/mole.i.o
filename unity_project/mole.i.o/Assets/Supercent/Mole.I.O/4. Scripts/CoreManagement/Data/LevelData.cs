using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] HammerLevel[] _hammerLevels;
        public int MaxHammerLevel => _hammerLevels.Length;
        public HammerLevel GetNextHammerLvData(int curlevel)
        {
            if (curlevel >= MaxHammerLevel)
                return _hammerLevels[MaxHammerLevel - 1];

            return _hammerLevels[curlevel];
        }

        [Serializable]
        public struct HammerLevel
        {
            public int RequireXp;
            public int AttackRange;
            public float PlayerSize;
            public int HammerModelType;
        }
    }
}