using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class LookAtCanvas : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}