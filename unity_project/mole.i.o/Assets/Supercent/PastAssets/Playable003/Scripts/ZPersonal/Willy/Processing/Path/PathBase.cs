using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public abstract class PathBase : MonoBehaviour
    {
        public abstract Vector3 Evaluate(float t);
        public abstract void OnDrawGizmos();
        public abstract Vector3 GetForwardAngle();
    }
}