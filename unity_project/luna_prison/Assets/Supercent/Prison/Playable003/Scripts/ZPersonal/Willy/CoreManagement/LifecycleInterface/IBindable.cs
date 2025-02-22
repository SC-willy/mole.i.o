using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public interface IBindable
    {
#if UNITY_EDITOR
        void Bind(MonoBehaviour mono);
#endif //UNITY_EDITOR
    }

}
