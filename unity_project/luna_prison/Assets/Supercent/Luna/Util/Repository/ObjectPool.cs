using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Supercent.Util
{
    public sealed class ObjectPool<T> : PoolBase<T> where T : Component, IPoolObject
    {
        public bool InactiveObject = true;

        public ObjectPool(T origin, Func<T, T> generator, int capacity) : base(origin, generator, Terminator, capacity) { }



        static void Terminator(T item)
        {
            if (item != null)
                UnityObject.Destroy(item.gameObject);
        }

        protected override void OnReturn(T item)
        {
            base.OnReturn(item);
            if (item == null)
                return;

            if (InactiveObject)
                item.gameObject.SetActive(false);
        }
    }
}