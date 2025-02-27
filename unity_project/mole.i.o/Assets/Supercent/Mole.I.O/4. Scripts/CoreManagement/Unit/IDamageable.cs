using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public interface IDamageable
    {
        public void GetDamage(int attackerLevel);
        public void GetWeakAttack();
        public void GetDeadlyAttack();
    }
}