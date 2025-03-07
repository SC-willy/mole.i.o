using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class ModifyStackerAtOnEnable : MonoBehaviour
    {
        [SerializeField] MonoStacker _stacker;
        [SerializeField] float _itemGetGapTime = 0;

        private void OnEnable()
        {
            _stacker.Stacker.Data.SetDuration(_itemGetGapTime);
            Destroy(this);
        }
    }
}