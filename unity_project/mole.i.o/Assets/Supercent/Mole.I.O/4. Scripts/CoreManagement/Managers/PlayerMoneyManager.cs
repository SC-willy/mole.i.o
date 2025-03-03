using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif // UNITY_EDITOR

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class PlayerMoneyManager : InstancedClass<PlayerMoneyManager>
    {
        int _money;
    }

}

