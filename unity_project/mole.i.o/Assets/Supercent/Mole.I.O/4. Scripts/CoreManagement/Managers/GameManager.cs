using System.Collections.Generic;
using Supercent.MoleIO.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    const string FIRST_CHECK = "IsFirst";
    static bool _isDynamicLoaded = false;
    static bool _isEssentialLoaded = false;
    public static string PlayerName => PlayerData.Name;

    static Dictionary<int, int> _dynamicDataDic = new Dictionary<int, int>();
    static GooglePlayManager _googlePlayManager = new GooglePlayManager();
    static SceneLoadingUI _sceneLoadingUI;

    static public void SetDynamicData(Dictionary<int, int> dynamicDataDic) => _dynamicDataDic = dynamicDataDic;
    static public float GetDynamicData(DynamicGameData.EDynamicType type)
    {
        if (_dynamicDataDic.ContainsKey((int)type))
            return _dynamicDataDic[(int)type];
        return -1f;
    }

    static public void SetLoadUI(SceneLoadingUI sceneLoadingUI) => _sceneLoadingUI = sceneLoadingUI;
    public static bool IsDynamicLoaded()
    {
        if (_isDynamicLoaded)
            return true;

        _isDynamicLoaded = true;
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

    public static bool IsFirst()
    {
        if (!PlayerPrefs.HasKey(FIRST_CHECK))
        {
            PlayerPrefs.SetInt(FIRST_CHECK, 0);
            return true;
        }
        return false;
    }

    public static void LoadLoadScene()
    {
        if (_sceneLoadingUI == null)
            return;

        _sceneLoadingUI.LoadScene();
    }
}
