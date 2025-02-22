
using UnityEngine;
using UnityEditor;

namespace Supercent.WillyUtil
{
    public class MissingScriptEraser : Editor
    {
        [MenuItem("Supercent/WillyUtil/Remove Missing Scripts")]
        public static void RemoveMissingScriptsa()
        {
            foreach (GameObject go in GetAllObjectsInScene(true))
            {
                RemoveMissingScriptsFromGameObject(go);
            }
        }

        static void RemoveMissingScriptsFromGameObject(GameObject go)
        {
            int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (removedCount > 0)
            {
                Debug.Log($"{removedCount} missing scripts removed from {go.name}", go);
            }

            foreach (Transform child in go.transform)
            {
                RemoveMissingScriptsFromGameObject(child.gameObject);
            }
        }

        static GameObject[] GetAllObjectsInScene(bool includeInactive)
        {
            return FindObjectsOfType<GameObject>(includeInactive);
        }
    }
}