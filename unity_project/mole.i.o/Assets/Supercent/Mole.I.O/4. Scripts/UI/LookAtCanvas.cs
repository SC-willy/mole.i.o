using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class LookAtCanvas : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}