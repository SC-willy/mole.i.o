using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Supercent.Util.Editor
{
    public static class ShadowDisabler
    {
        [MenuItem("GameObject/Supercent/Disable Shadows", true)]
        static bool ValidateDisableShadowsFromSelectedObject() => Selection.activeGameObject != null;
        [MenuItem("GameObject/Supercent/Disable Shadows", false, 10)]
        static void DisableShadowsFromSelectedObject()
        {
            var obj = Selection.activeGameObject;
            if (obj != null)
                DisableShadowsJob(obj.GetComponentsInChildren<Renderer>());
        }

        [MenuItem("GameObject/Supercent/Disable Shadows in Scene", true)]
        static bool ValidateDisableShadowsFromScene() => Selection.activeGameObject == null;
        [MenuItem("GameObject/Supercent/Disable Shadows in Scene", false, 11)]
        static void DisableShadowsFromScene()
        {
            if (Selection.activeGameObject == null)
                DisableShadowsJob(Object.FindObjectsOfType<Renderer>());
        }

        static void DisableShadowsJob(Renderer[] renderers)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                EditorUtility.SetDirty(renderer);
                PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
            }

            Debug.Log($"Disabled shadows for {renderers.Length} renderers.");
        }
    }
}