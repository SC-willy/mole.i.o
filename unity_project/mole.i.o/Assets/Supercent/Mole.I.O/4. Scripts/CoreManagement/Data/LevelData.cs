using System;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level")]
    public class LevelData : ScriptableObject
    {
        const int HammerLevelCount = 10;
        const float MAX_RAND_LEVEL_VALUE = 1.1f;
        public int MaxHammerLevel => _hammerLevels.Length;
        [SerializeField] HammerLevel[] _hammerLevels;

        public void SetHammerInfo()
        {
            if (_hammerLevels.Length < HammerLevelCount)
                _hammerLevels = new HammerLevel[HammerLevelCount];

            for (int i = 0; i < HammerLevelCount; i++)
            {
                HammerLevel curHammer = new HammerLevel();
                curHammer.RequireXp = (int)GameManager.GetLevelData(GameManager.EDynamicType.XpPerLevel, i);
                curHammer.AttackRange = (int)GameManager.GetLevelData(GameManager.EDynamicType.RangePerLevel, i);
                curHammer.PlayerSize = (int)GameManager.GetLevelData(GameManager.EDynamicType.SizePerLevel, i);
                curHammer.HammerModelType = (int)GameManager.GetLevelData(GameManager.EDynamicType.HammerModelPerLevel, i);
                _hammerLevels[i] = curHammer;
            }
        }

        public HammerLevel GetNextHammerLvData(int curlevel)
        {
            if (curlevel >= MaxHammerLevel)
                return _hammerLevels[MaxHammerLevel - 1];

            return _hammerLevels[curlevel];
        }

        public int EvaluateXpToLevel(int xp)
        {
            for (int i = 0; i < _hammerLevels.Length; i++)
            {
                if (_hammerLevels[i].RequireXp > xp)
                    return Mathf.Max(i - 1, 0);
            }
            return MaxHammerLevel;
        }

        public int EvaluateLevelToRandomXp(int level)
        {
            if (level >= _hammerLevels.Length)
                level = _hammerLevels.Length - 1;

            int randXpGap;
            if (level >= _hammerLevels.Length - 1)
                randXpGap = (int)(_hammerLevels[level].RequireXp * MAX_RAND_LEVEL_VALUE);
            else
                randXpGap = _hammerLevels[level + 1].RequireXp - _hammerLevels[level].RequireXp - 1;

            return _hammerLevels[level].RequireXp + UnityEngine.Random.Range(0, randXpGap);
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