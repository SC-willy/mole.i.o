using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public abstract class PathBase : MonoBehaviour
    {
        public abstract Vector3 Evaluate(float t);
        public abstract void OnDrawGizmos();
        public abstract Vector3 GetForwardAngle();
    }
}