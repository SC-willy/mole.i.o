using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class ReleaseArea : MonoStacker
    {
        StackableItem _item;

        [Space]
        public bool CanUpdate = false;

        void OnTriggerStay(Collider other)
        {
            var inventory = StackerLib.Get(other.gameObject);

            if (!inventory.IsCanGet)
                return;

            if (!_stacker.TryGiveItem(out _item))
                return;

            inventory.TryGetItem(_item);
        }
    }
}