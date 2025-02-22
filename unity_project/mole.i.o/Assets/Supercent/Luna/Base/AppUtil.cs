using System;
using UnityEngine;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;

namespace Supercent.Base
{
    public static class AppUtil
    {
        #region Object
        public static GameObject GetAppObejct()
        {
            var name = "APP";
            if (!FindRoot(name, out var objApp))
                objApp = new GameObject(name);

            objApp.SetActive(true);
            return objApp;
        }

        public static bool FindRoot(string name, out GameObject result)
        {
            var items = UnityObject.FindObjectsOfType<GameObject>();
            if (items != null)
            {
                for (int index = 0, cnt = items.Length; index < cnt; ++index)
                {
                    var item = items[index];
                    if (string.CompareOrdinal(item.name, name) == 0)
                    {
                        result = item;
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        public static bool GetOrAddComponent<T>(Component src, out T result) where T : Component
        {
            result = null;
            if (src == null) return false;

            if (!src.TryGetComponent(out result))
                result = src.gameObject.AddComponent<T>();
            return true;
        }

        public static bool GetOrAddComponent<T>(Component root, string name, out T result) where T : Component
        {
            result = null;
            if (root == null)
                return false;

            var src = root.transform.Find(name);
            if (src == null)
                src = NewObject(name, root).transform;
            return GetOrAddComponent(src, out result);
        }

        public static GameObject NewObject(string name, Component parent)
        {
            var result = new GameObject(name, typeof(RectTransform));
            var transform = result.transform;
            if (parent != null)
                transform.SetParent(parent.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            return result;
        }
        #endregion// Object


        #region Transform
        public enum AnchorType
        {
            LeftTop = 0,
            LeftMiddle,
            LeftBottom,

            CenterTop,
            CenterMiddle,
            CenterBottom,

            RightTop,
            RightMiddle,
            RightBottom,

            LeftStretch,
            CenterStretch,
            RightStretch,

            TopStretch,
            MiddleStretch,
            BottomStretch,

            FullStretch,
        }

        public enum PivotType
        {
            LeftTop = 0,
            LeftMiddle,
            LeftBottom,

            CenterTop,
            CenterMiddle,
            CenterBottom,

            RightTop,
            RightMiddle,
            RightBottom,
        }

        static readonly Vector2 ZeroHalf = new Vector2(0f, 0.5f);
        static readonly Vector2 HalfOne = new Vector2(0.5f, 1f);
        static readonly Vector2 Half = new Vector2(0.5f, 0.5f);
        static readonly Vector2 HalfZero = new Vector2(0.5f, 0f);
        static readonly Vector2 OneHalf = new Vector2(1f, 0.5f);

        static readonly Action<RectTransform>[] AnchorJob = new Action<RectTransform>[]
        {
            src => { src.anchorMin = Vector2.up; src.anchorMax = Vector2.up; },
            src => { src.anchorMin = ZeroHalf; src.anchorMax = ZeroHalf; },
            src => { src.anchorMin = Vector2.zero; src.anchorMax = Vector2.zero; },

            src => { src.anchorMin = HalfOne; src.anchorMax = HalfOne; },
            src => { src.anchorMin = Half; src.anchorMax = Half; },
            src => { src.anchorMin = HalfZero; src.anchorMax = HalfZero; },

            src => { src.anchorMin = Vector2.one; src.anchorMax = Vector2.one; },
            src => { src.anchorMin = OneHalf; src.anchorMax = OneHalf; },
            src => { src.anchorMin = Vector2.right; src.anchorMax = Vector2.right; },

            src => { src.anchorMin = Vector2.zero; src.anchorMax = Vector2.up; },
            src => { src.anchorMin = HalfZero; src.anchorMax = HalfOne; },
            src => { src.anchorMin = Vector2.right; src.anchorMax = Vector2.one; },

            src => { src.anchorMin = Vector2.up; src.anchorMax = Vector2.one; },
            src => { src.anchorMin = ZeroHalf; src.anchorMax = OneHalf; },
            src => { src.anchorMin = Vector2.zero; src.anchorMax = Vector2.right; },

            src => { src.anchorMin = Vector2.zero; src.anchorMax = Vector2.one; },
        };

        public static void SetAnchor(this RectTransform src, AnchorType type) => AnchorJob[(int)type](src);
        public static void SetAnchor(this Graphic src, AnchorType type) => AnchorJob[(int)type](src.rectTransform);
        public static void SetAnchor(this RectTransform src, Rect rect) { src.anchorMin = rect.min; src.anchorMax = rect.max; }
        public static void SetAnchor(this Graphic src, Rect rect) => SetAnchor(src.rectTransform, rect);


        static readonly Action<RectTransform>[] PivotJob = new Action<RectTransform>[]
        {
            src => src.pivot = Vector2.up,
            src => src.pivot = ZeroHalf,
            src => src.pivot = Vector2.zero,

            src => src.pivot = HalfOne,
            src => src.pivot = Half,
            src => src.pivot = HalfZero,

            src => src.pivot = Vector2.one,
            src => src.pivot = OneHalf,
            src => src.pivot = Vector2.right,
        };

        public static void SetPivot(this RectTransform src, PivotType type) => PivotJob[(int)type](src);
        public static void SetPivot(this Graphic src, PivotType type) => SetPivot(src.rectTransform, type);

        public static void SetFullStretch(this RectTransform src)
        {
            src.anchorMin = Vector2.zero;
            src.anchorMax = Vector2.one;
            src.sizeDelta = Vector2.zero;
            src.anchoredPosition = Vector2.zero;
        }
        public static void SetFullStretch(this Graphic src) => SetFullStretch(src.rectTransform);
        #endregion// Transform
    }
}