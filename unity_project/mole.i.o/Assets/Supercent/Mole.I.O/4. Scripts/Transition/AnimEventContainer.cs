using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class AnimEventContainer : MonoBehaviour
    {
        public event Action OnAnimEvent;

        public void InvokeAnimEvent() => OnAnimEvent?.Invoke();
    }
}