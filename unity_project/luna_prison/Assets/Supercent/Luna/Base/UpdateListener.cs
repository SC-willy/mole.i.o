using System;
using UnityEngine;

namespace Supercent.Base
{
    public class UpdateListener : MonoBehaviour
    {
        public event Action update;
        public event Action lateUpdate;


        void OnDestroy()
        {
            update = null;
            lateUpdate = null;
        }

        void Update()
        {
            if (update != null)
                update();
        }
        void LateUpdate()
        {
            if (lateUpdate != null)
                lateUpdate();
        }
    }
}