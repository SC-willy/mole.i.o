using System.Collections;
using System.Collections.Generic;
using Supercent.MoleIO.Management;
using UnityEngine;

public static class GameManager
{
    const string FIRST_CHECK = "IsFirst";
    static bool _isLoaded = false;
    static string _playerName = "Player";
    public static string PlayerName => _playerName;

    public static bool IsLoaded()
    {
        if (_isLoaded)
            return true;

        _isLoaded = true;
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
}
