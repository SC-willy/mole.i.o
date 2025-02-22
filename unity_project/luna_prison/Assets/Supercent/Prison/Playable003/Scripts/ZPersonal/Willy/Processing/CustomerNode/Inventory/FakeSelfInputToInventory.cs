using System.Collections;
using Supercent.Util;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class FakeSelfInputToInventory : SelfInputToInventory
    {
        [SerializeField] ObjectPoolHandler _pool;
        protected override void OnCollisionEnter(Collision other)
        {
            if (PLAYER_LAYER_NUM != other.gameObject.layer)
                return;

            if (_inventory == null)
            {
                _inventory = StackerLib.Get(other.gameObject);
                if (_inventory == null)
                    return;
            }

            if (_inventory.IsCanGet)
            {
                _itemInfo.ExecuteOnStack();
                StackableItem item = _pool.Get() as StackableItem;
                item.RerollVariants();
                item.transform.position = transform.position;
                _inventory.TryGetItem(item);
            }

            AciveRigidBody();
        }

        private void OnCollisionStay(Collision other)
        {
            OnCollisionEnter(other);
        }
    }
}