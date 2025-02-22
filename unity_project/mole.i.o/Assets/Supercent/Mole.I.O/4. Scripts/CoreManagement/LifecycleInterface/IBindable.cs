using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public interface IBindable
    {
#if UNITY_EDITOR
        void Bind(MonoBehaviour mono);
#endif //UNITY_EDITOR
    }

}
