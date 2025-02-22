using UnityEngine;

namespace Supercent.Util
{
    public static class HierarchyExtensions
    {
        #region Layer
        public static bool HasLayer(this LayerMask mask, int layer)
        {
            var layerValue = 1 << layer;
            return 0 != (layerValue & mask);
        }

        public static void SetLayerWithChildren(this GameObject gobj, string layerName)
        {
            if (null == gobj) { Debug.LogError("[SetLayerWithChildren] This object is null !"); return; }

            SetLayerWithChildren(gobj.transform, LayerMask.NameToLayer(layerName));
        }
        public static void SetLayerWithChildren(this GameObject gobj, int layer)
        {
            if (null == gobj) { Debug.LogError("[SetLayerWithChildren] This object is null !"); return; }

            SetLayerWithChildren(gobj.transform, layer);
        }
        public static void SetLayerWithChildren(this Component gobj, string layerName)
        {
            if (null == gobj) { Debug.LogError("[SetLayerWithChildren] This object is null !"); return; }

            SetLayerWithChildren(gobj.transform, LayerMask.NameToLayer(layerName));
        }
        public static void SetLayerWithChildren(this Component gobj, int layer)
        {
            if (null == gobj) { Debug.LogError("[SetLayerWithChildren] This object is null !"); return; }

            SetLayerWithChildren(gobj.transform, layer);
        }
        static void SetLayerWithChildren(Transform gt, int layer)
        {
            if (null == gt)
                return;

            gt.gameObject.layer = layer;

            var childCount = gt.childCount;
            if (childCount <= 0)
                return;

            for (int n = 0, cnt = gt.childCount; n < cnt; ++n)
            {
                var child = gt.GetChild(n);
                if (null != child)
                    SetLayerWithChildren(child, layer);
            }
        }
        #endregion// Layer
    }
}