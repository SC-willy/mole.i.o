using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class ComponentRegistryField<T>
    {
        [SerializeField] protected T[] _components;
        public T[] Components => _components;

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _components = mono.GetComponentsInChildren<T>(true);
        }
#endif // UNITY_EDITOR

    }

}