using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public static class MathUtil
    {
        public static Vector3 RandomOnCircle(float radius = 1f)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }
    }
}