using UnityEditor;
using UnityEngine;

namespace Supercent.Rendering.Shadow.Editor
{
    public static class PlanarShadowEditorUtility
    {
        public static void LoadAllPrefabs(System.Action<GameObject> onPrefabLoaded)
        {
            string[] assetPaths = AssetDatabase.FindAssets("t:Prefab");
            int totalAssets = assetPaths.Length;

            for (int i = 0; i < totalAssets; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[i]);

                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (asset == null)
                {
                    Debug.LogWarning($"<color=yellow>[Planar Shadow] 다음 경로에 대한 어셋을 로드하는데 실패하였습니다. {assetPath}</color>");
                    continue;
                }

                onPrefabLoaded?.Invoke(asset);
            }
        }

        public static Material GetPlanarShadowOriginalMat()
        {
            string path = "PlanarShadowMat";
            Material planarShadowOriginalMat = Resources.Load<Material>(path);

            if (planarShadowOriginalMat == null)
                Debug.LogError($"<color=red>[Planar Shadow] Assets/Resources/{path}.mat에 그림자 머티리얼이 없습니다!</color>");

            return planarShadowOriginalMat;
        }

        public static Material GetPlanarShadowBakedMat()
        {
            string path = "PlanarShadowBakedMat";
            Material planarShadowOriginalMat = Resources.Load<Material>(path);

            if (planarShadowOriginalMat == null)
                Debug.LogError($"<color=red>[Planar Shadow] Assets/Resources/{path}.mat에 그림자 머티리얼이 없습니다!</color>");

            return planarShadowOriginalMat;
        }

        public static bool HasMissingScripts(GameObject target)
        {
            Component[] components = target.GetComponentsInChildren<Component>(true);
            foreach (var component in components)
            {
                if (component == null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}