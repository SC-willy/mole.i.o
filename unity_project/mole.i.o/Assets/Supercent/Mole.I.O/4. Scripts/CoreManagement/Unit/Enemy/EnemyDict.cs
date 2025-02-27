using System.Collections;
using System.Collections.Generic;
using Supercent.MoleIO.InGame;
using UnityEngine;


public class ColDict
{
    public static ColDict Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ColDict();

            return _instance;
        }

    }

    private static ColDict _instance;
    private Dictionary<Collider, IDamageable> _dict = new Dictionary<Collider, IDamageable>();

    public static IDamageable GetData(Collider collider)
    {
        return Instance._GetData(collider);
    }

    public IDamageable _GetData(Collider collider)
    {
        if (_dict.TryGetValue(collider, out IDamageable controller))
            return controller;
        return null;
    }

    public static void RegistData(Collider collider, IDamageable controller)
    {
        Instance._RegistData(collider, controller);
    }

    public void _RegistData(Collider collider, IDamageable controller)
    {
        _dict.Add(collider, controller);
    }
}
