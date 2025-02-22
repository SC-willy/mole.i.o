using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public abstract class ComponentRegistry<T> : ComponentRegistryBase
    {
        [SerializeField] ComponentRegistryField<T> _registryData = new ComponentRegistryField<T>();

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            _registryData.Bind(this);
        }
#endif // UNITY_EDITOR
    }

}