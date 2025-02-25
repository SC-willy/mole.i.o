using System.Collections;
using System.Collections.Generic;
using Supercent.MoleIO.InGame;
using UnityEngine;


public class EnemyDict
{
    public static EnemyDict Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EnemyDict();

            return _instance;
        }

    }

    private static EnemyDict _instance;
    private Dictionary<Collider, EnemyController> _dict = new Dictionary<Collider, EnemyController>();

    public static EnemyController GetData(Collider collider)
    {
        return Instance._GetData(collider);
    }

    public EnemyController _GetData(Collider collider)
    {
        if (_dict.TryGetValue(collider, out EnemyController controller))
            return controller;
        return null;
    }

    public static void RegistData(Collider collider, EnemyController controller)
    {
        Instance._RegistData(collider, controller);
    }

    public void _RegistData(Collider collider, EnemyController controller)
    {
        _dict.Add(collider, controller);
    }
}
