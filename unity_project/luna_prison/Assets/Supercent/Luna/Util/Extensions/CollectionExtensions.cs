using System;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace Supercent.Util
{
    public static class CollectionExtensions
    {
        public static bool IsWithinRange<T>(this IList<T> src, int index)
        {
            if (src == null)
            {
                Debug.LogError($"{nameof(IsWithinRange)} : {nameof(src)}가 null 입니다.");
                return default;
            }

            return 0 <= index && index < src.Count;
        }
        public static bool IsOutOfRange<T>(this IList<T> src, int index)
        {
            if (src == null)
            {
                Debug.LogError($"{nameof(IsOutOfRange)} : {nameof(src)}가 null 입니다.");
                return default;
            }

            return index < 0 || src.Count <= index;
        }

        public static bool TryGetValue<T>(this IList<T> list, int index, out T value)
        {
            value = default;
            {
                if (null == list) return false;
                if (index < 0) return false;
                if (list.Count <= index) return false;

                value = list[index];
            }
            return true;
        }

        public static T GetOrNew<K, T>(this IDictionary<K, T> src, K key) where T : class, new()
        {
            if (src == null)
            {
                Debug.LogError($"{nameof(GetOrNew)} : {nameof(src)}가 null 입니다.");
                return default;
            }

            if (!src.TryGetValue(key, out var value))
                src.Add(key, value = new T());
            return value;
        }
        public static T[] GetOrNewArray<K, T>(this IDictionary<K, T[]> src, K key, int length) where T : class, new()
        {
            if (src == null)
            {
                Debug.LogError($"{nameof(GetOrNewArray)} : {nameof(src)}가 null 입니다.");
                return default;
            }

            if (!src.TryGetValue(key, out var value))
                src.Add(key, value = new T[length]);
            return value;
        }

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str) || string.CompareOrdinal(str, "null") == 0;
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => null == collection || collection.Count <= 0;



        #region Shuffle
        public static void Shuffle<T>(IList<T> list, int? seed = null)
        {
            // HACK : Unity에서 Array 유형이 IsReadOnly로 반환되는 버그가 있어 조건 추가함
            if (list.IsReadOnly && !(list is T[]))
            {
                Debug.LogError($"{nameof(Shuffle)}<{typeof(T).Name}> : {nameof(list)}가 읽기 전용 입니다");
                return;
            }

            if (list.Count < 2)
                return;

            var oldState = UnityRandom.state;
            UnityRandom.InitState(seed.HasValue ? seed.Value : (int)DateTime.UtcNow.Ticks);
            {
                for (int index = list.Count; 1 < index; --index)
                {
                    int randIndex = UnityRandom.Range(0, index);
                    var setIndex = index - 1;
                    if (randIndex == setIndex)
                    {
                        if ((setIndex & 1) == 1)
                            continue;
                        randIndex = setIndex - 1;
                    }

                    T tmp = list[randIndex];
                    list[randIndex] = list[setIndex];
                    list[setIndex] = tmp;
                }
            }
            UnityRandom.state = oldState;
        }
        #endregion// Shuffle
    }
}