using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class InputArea : MonoStacker
    {
        public Action OnInput;

        [Space]
        [SerializeField] EStackableItemType _type = EStackableItemType.None;
        public bool CanUpdate = true;
        StackableItem _stackableItem;
        Stacker _others;

        void OnTriggerStay(Collider other)
        {
            if (!_stacker.IsCanGet) return;

            if (_others == null)
            {
                _others = StackerLib.Get(other.gameObject);
            }

            if (!_others.TryFilterdGiveItem((int)_type, out _stackableItem)) return;
            if (!CanUpdate) return;
            _stacker.TryGetItem(_stackableItem);

            //직원 상호작용 필요 시
            //_others = null
        }
    }
}