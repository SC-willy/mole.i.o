using System.Collections.Generic;
using Supercent.MoleIO.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public enum EDynamicType
    {
        LevelPerUpgrade = 1001,
        AtkRateReducePerUpgrade,
        AtkRateReduceMax,
        SpeedPerUpgrade,
        CostPerUpgrade,
        PlayerSpeeed,
        PlayerAtkRate,
        AiSpeed,
        AiAtkRate,
        PlayTime,
        CamZoom,
        StunTime,
        AiLevelMin,
        AiLevelMax,

        RangePerLevel = 1100,
        XpPerLevel = 1200,
        SizePerLevel = 1300,
        HammerModelPerLevel = 1400,
        BossHpPerStage = 2000,
        EnemyCountPerStage = 2100,
    }

    const string FIRST_CHECK = "IsFirst";
    static bool _isDynamicLoaded = false;
    static bool _isEssentialLoaded = false;
    public static string PlayerName => PlayerData.Name;
    public static bool IsDynamicLoaded => _isDynamicLoaded;

    static Dictionary<int, float> _dynamicDataDic = new Dictionary<int, float>();
    static GooglePlayManager _googlePlayManager = new GooglePlayManager();
    static SceneLoadingUI _sceneLoadingUI;

    #region  System
    static public void SetLoadUI(SceneLoadingUI sceneLoadingUI) => _sceneLoadingUI = sceneLoadingUI;
    public static bool IsFirst()
    {
        if (!PlayerPrefs.HasKey(FIRST_CHECK))
        {
            PlayerPrefs.SetInt(FIRST_CHECK, 0);
            return true;
        }
        return false;
    }
    public static void LoadEssentials()
    {
        if (_isEssentialLoaded)
            return;

        _isEssentialLoaded = true;

        _googlePlayManager.StartSetup();
        PlayerData.LoadData();
        return;
    }

    public static void LoadScene()
    {
        if (_sceneLoadingUI == null)
            return;

        _sceneLoadingUI.LoadScene();
    }

    #endregion

    #region Data
    static public void SetDynamicData(Dictionary<int, float> dynamicDataDic)
    {
        _dynamicDataDic = dynamicDataDic;
        _isDynamicLoaded = true;
    }
    static public void SetBakedDynamicData(Dictionary<int, float> dynamicDataDic)
    {
        _dynamicDataDic = dynamicDataDic;
        _isDynamicLoaded = true;
    }
    static public float GetDynamicData(EDynamicType type) => GetDynamicData((int)type);
    static public float GetDynamicData(int code)
    {
        if (_dynamicDataDic.ContainsKey(code))
            return _dynamicDataDic[code];
        return -1f;
    }
    static public float GetLevelData(EDynamicType type, int level) => GetLevelData((int)type, level);
    static public float GetLevelData(int code, int level)
    {
        if (_dynamicDataDic.ContainsKey(code + level))
            return _dynamicDataDic[code];
        return -1f;
    }
    #endregion

}
