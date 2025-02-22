using System.Collections.Generic;
using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class StackerLib : InitManagedBehaviorBase
    {
        private static Dictionary<GameObject, Stacker> _invenDictionary = new Dictionary<GameObject, Stacker>();
        [SerializeField] MonoStacker[] _stackers;

        protected override void _Init()
        {
            for (int i = 0; i < _stackers.Length; i++)
            {
                _invenDictionary.Add(_stackers[i].gameObject, _stackers[i].Stacker);
            }
            _stackers = null;
        }
        public static Stacker Get(GameObject gameObject)
        {
            if (_invenDictionary.TryGetValue(gameObject, out Stacker inventory))
            {
                return inventory;
            }
            return null;
        }

        public static bool Set(GameObject gameObject, Stacker inventory) => _invenDictionary.TryAdd(gameObject, inventory);

        protected override void _Release()
        {
            _invenDictionary = null;
            _stackers = null;
        }

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            _stackers = GetComponentsInChildren<MonoStacker>(true);
        }
#endif //UNITY_EDITOR
    }


}