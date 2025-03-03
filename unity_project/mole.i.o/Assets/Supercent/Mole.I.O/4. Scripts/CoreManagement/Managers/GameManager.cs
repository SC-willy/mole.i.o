using Supercent.MoleIO.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    const string FIRST_CHECK = "IsFirst";
    const string LOBBY_SCENE = "Lobby";
    const int STAGE_LENGTH = 1;
    static bool _isLoaded = false;
    public static string PlayerName => PlayerData.Name;

    static GooglePlayManager _googlePlayManager = new GooglePlayManager();
    public static bool IsLoaded()
    {
        if (_isLoaded)
            return true;

        _isLoaded = true;

        _googlePlayManager.StartSetup();
        PlayerData.LoadData();
        return false;
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

    public static void LoadGameScene()
    {
        if (STAGE_LENGTH <= PlayerData.Stage)
            SceneManager.LoadScene($"Stage00{STAGE_LENGTH - 1}");
        else
            SceneManager.LoadScene($"Stage00{PlayerData.Stage}");
    }

    public static void LoadLobbyScene()
    {
        SceneManager.LoadScene(LOBBY_SCENE);
        
    }
}
