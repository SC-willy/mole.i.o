using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class BossFlowManager : IStartable
    {
        [SerializeField] GameObject _bossFlowObjs;
        [SerializeField] ImageTouchChecker _btn;
        public void StartSetup()
        {
            _btn.OnPointerDownEvent += Charge;
        }
        private void Charge()
        {

        }


    }
}