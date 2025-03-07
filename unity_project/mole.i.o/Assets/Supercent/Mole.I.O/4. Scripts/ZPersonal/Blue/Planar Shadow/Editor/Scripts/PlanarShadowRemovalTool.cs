using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Supercent.Rendering.Shadow.Editor
{
    public static class PlanarShadowRemovalTool
    {
        #region 그림자 제거
        private static class PlanarShadowCleaner
        {
            [MenuItem("Supercent/Planar Shadow/그림자 기능 제거 도구/🚨 모든 그림자 제거 (어셋 폴더 내 프리팹 및 모든 씬 오브젝트)", false, 10)]
            private static void CleanAllShadows()
            {
                if (false == EditorUtility.DisplayDialog("작업 수행 경고", "해당 작업은 수행 후 복구할 수 없습니다. 진행하십니까?", "네", "아니오"))
                    return;

                try
                {
                    EditorUtility.DisplayProgressBar("그림자 제거", "에셋 폴더의 프리팹을 처리 중...", 0.0f);
                    CleanShadowOnAssetsFolder();

                    EditorUtility.DisplayProgressBar("그림자 제거", "씬 파일을 처리 중...", 0.5f);
                    CleanShadowOnScenes();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                EditorUtility.DisplayDialog("그림자 제거 완료", "모든 그림자 관련 요소가 제거되었습니다.", "확인");

                AssetDatabase.Refresh();
            }

            private static void CleanShadowOnAssetsFolder()
            {
                string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
                int total = prefabGuids.Length;
                int processed = 0;

                foreach (var guid in prefabGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    if (asset == null)
                    {
                        Debug.LogError($"<color=red>[Planar Shadow] 프리팹 로드 실패: {path}</color>");
                        continue;
                    }

                    CleanShadowInGameObject(asset, true, path);
                    processed++;
                    EditorUtility.DisplayProgressBar("그림자 제거", $"에셋 처리 중... ({processed}/{total})", (float)processed / total);
                }
            }

            private static void CleanShadowOnScenes()
            {
                Scene currentScene = SceneManager.GetActiveScene();
                string currentScenePath = currentScene.path;
                bool isCurrentSceneDirty = currentScene.isDirty;

                if (!string.IsNullOrEmpty(currentScenePath))
                    if (isCurrentSceneDirty)
                        if (EditorUtility.DisplayDialog("씬 변경 사항 저장", "현재 열린 씬에 변경 사항이 있습니다. 저장하시겠습니까?", "저장", "취소"))
                            EditorSceneManager.SaveScene(currentScene);

                string[] allScenePaths = AssetDatabase.FindAssets("t:Scene")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .ToArray();

                int total = allScenePaths.Length;
                int processed = 0;

                foreach (string scenePath in allScenePaths)
                {
                    Scene loadedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    GameObject[] rootGameObjects = loadedScene.GetRootGameObjects();

                    foreach (var rootObject in rootGameObjects)
                    {
                        CleanShadowInGameObject(rootObject, false, scenePath);
                    }

                    EditorSceneManager.SaveScene(loadedScene);
                    processed++;
                    EditorUtility.DisplayProgressBar("그림자 제거", $"씬 처리 중... ({processed}/{total})", 0.5f + (float)processed / total * 0.5f);
                }

                if (!string.IsNullOrEmpty(currentScenePath))
                    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
            }

            private static void CleanShadowInGameObject(GameObject target, bool isPrefab, string assetPath)
            {
                PlanarShadow[] planarShadows = target.GetComponentsInChildren<PlanarShadow>(true);
                bool isModified = false;

                foreach (var planarShadow in planarShadows)
                {
                    if (PrefabUtility.IsPartOfPrefabInstance(planarShadow.gameObject))
                    {
                        if (PrefabUtility.HasPrefabInstanceAnyOverrides(planarShadow.gameObject, false))
                        {
                            Debug.Log($"<color=yellow>[Planar Shadow] {planarShadow.gameObject.name} - 프리팹 인스턴스에서 그림자 제거</color>");
                            planarShadow.Editor_RemovePlanarShadowMaterial();
                            GameObject.DestroyImmediate(planarShadow, true);
                            isModified = true;
                        }
                        continue;
                    }

                    Debug.Log($"<color=yellow>[Planar Shadow] {planarShadow.gameObject.name} - 그림자 제거</color>");
                    planarShadow.Editor_RemovePlanarShadowMaterial();
                    GameObject.DestroyImmediate(planarShadow, true);
                    isModified = true;
                }

                Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true)
                    .Where(r => r is MeshRenderer || r is SkinnedMeshRenderer)
                    .ToArray();

                foreach (var renderer in renderers)
                {
                    if (PrefabUtility.IsPartOfPrefabInstance(renderer.gameObject))
                    {
                        if (PrefabUtility.HasPrefabInstanceAnyOverrides(renderer.gameObject, false))
                        {
                            Material[] originalMaterials = renderer.sharedMaterials;

                            Material[] filteredMaterials = originalMaterials
                                .Where(m => m == null || (m.shader != PlanarShadowEditorUtility.GetPlanarShadowBakedMat().shader && m.shader != PlanarShadowEditorUtility.GetPlanarShadowOriginalMat().shader))
                                .ToArray();

                            if (!originalMaterials.SequenceEqual(filteredMaterials))
                            {
                                Debug.Log($"<color=yellow>[Planar Shadow] {renderer.gameObject.name} - 머티리얼 변경</color>");
                                renderer.sharedMaterials = filteredMaterials;
                                isModified = true;
                            }
                        }
                        continue;
                    }

                    if (renderer.sharedMaterials.Any(m => m != null && m.shader == PlanarShadowEditorUtility.GetPlanarShadowBakedMat().shader))
                    {
                        Debug.Log($"<color=yellow>[Planar Shadow] {renderer.gameObject.name} - 그림자 머티리얼 포함, 제거됨</color>");
                        GameObject.DestroyImmediate(renderer.gameObject, true);
                        isModified = true;
                    }
                }

                if (isModified)
                {
                    if (isPrefab)
                    {
                        bool hasMissingScripts = PlanarShadowEditorUtility.HasMissingScripts(target);
                        if (hasMissingScripts)
                        {
                            Debug.LogError($"<color=red>[Planar Shadow] 프리팹 저장 실패 (Missing Script 존재): {assetPath}</color>");
                            return;
                        }

                        bool isRootEmptyObject = target.transform.childCount == 0 && target.GetComponents<Component>().Length == 1;
                        if (isRootEmptyObject)
                        {
                            AssetDatabase.DeleteAsset(assetPath);
                            Debug.Log($"<color=red>[Planar Shadow] 프리팹 삭제됨: {assetPath}</color>");
                        }
                        else
                        {
                            PrefabUtility.SavePrefabAsset(target);
                            Debug.Log($"<color=cyan>[Planar Shadow] 프리팹 갱신됨: {assetPath}</color>");
                        }
                    }
                }
            }
        }
        #endregion

        #region 그림자 존재 여부 검사
        private class PlanarShadowCheckTool : EditorWindow
        {
            private static Dictionary<GameObject, string> _foundShadows;
            private Vector2 _scrollPosition;

            [MenuItem("Supercent/Planar Shadow/그림자 기능 제거 도구/어셋 폴더 내 그림자 존재 여부 검사", false, 11)]
            private static void CheckShadowInAssetFolder()
            {
                Dictionary<GameObject, string> foundShadows = new();

                string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
                int totalPrefabs = prefabGuids.Length;
                int processedPrefabs = 0;

                try
                {
                    foreach (var guid in prefabGuids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                        if (asset == null)
                        {
                            Debug.LogError($"<color=red>[Planar Shadow] 프리팹 로드 실패: {path}</color>");
                            continue;
                        }

                        CheckShadowInGameObject(asset, path, foundShadows);
                        processedPrefabs++;
                        EditorUtility.DisplayProgressBar("그림자 여부 검증 (어셋 폴더)", $"프리팹 검사 중... ({processedPrefabs}/{totalPrefabs})", (float)processedPrefabs / totalPrefabs);
                    }
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                if (foundShadows.Count > 0)
                {
                    PlanarShadowCheckTool.ShowWindow(foundShadows);
                }
                else
                {
                    EditorUtility.DisplayDialog("그림자 여부 검증 완료", "어셋 폴더에서 PlanarShadow 및 그림자 머티리얼을 사용하는 오브젝트가 발견되지 않았습니다.", "확인");
                }
            }

            [MenuItem("Supercent/Planar Shadow/그림자 기능 제거 도구/현재 씬의 그림자 존재 여부 검사", false, 12)]
            private static void CheckShadowInCurrentScene()
            {
                Dictionary<GameObject, string> foundShadows = new();

                Scene currentScene = SceneManager.GetActiveScene();
                string currentScenePath = currentScene.path;
                bool isCurrentSceneDirty = currentScene.isDirty;

                try
                {
                    if (!string.IsNullOrEmpty(currentScenePath))
                    {
                        if (isCurrentSceneDirty)
                        {
                            if (EditorUtility.DisplayDialog("씬 변경 사항 저장", "현재 열린 씬에 변경 사항이 있습니다. 저장하시겠습니까?", "저장", "취소"))
                            {
                                EditorSceneManager.SaveScene(currentScene);
                            }
                        }

                        GameObject[] rootGameObjects = currentScene.GetRootGameObjects();
                        int totalObjects = rootGameObjects.Length;
                        int processedObjects = 0;

                        foreach (var rootObject in rootGameObjects)
                        {
                            CheckShadowInGameObject(rootObject, currentScenePath, foundShadows);
                            processedObjects++;
                            EditorUtility.DisplayProgressBar("그림자 여부 검증 (현재 씬)", $"오브젝트 검사 중... ({processedObjects}/{totalObjects})", (float)processedObjects / totalObjects);
                        }
                    }
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                if (foundShadows.Count > 0)
                {
                    PlanarShadowCheckTool.ShowWindow(foundShadows);
                }
                else
                {
                    EditorUtility.DisplayDialog("그림자 여부 검증 완료", "현재 씬에서 PlanarShadow 및 그림자 머티리얼을 사용하는 오브젝트가 발견되지 않았습니다.", "확인");
                }
            }

            private static void CheckShadowInGameObject(GameObject target, string path, Dictionary<GameObject, string> foundShadows)
            {
                PlanarShadow[] planarShadows = target.GetComponentsInChildren<PlanarShadow>(true);
                if (planarShadows.Length > 0)
                {
                    foreach (var planarShadow in planarShadows)
                    {
                        string message = $"PlanarShadow 컴포넌트 포함: {planarShadow.gameObject.name} (경로: {path})";
                        Debug.LogWarning($"<color=yellow>[Planar Shadow] {message}</color>", planarShadow.gameObject);
                        foundShadows[planarShadow.gameObject] = message;
                    }
                }

                Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true)
                    .Where(r => r is MeshRenderer || r is SkinnedMeshRenderer)
                    .ToArray();

                foreach (var renderer in renderers)
                {
                    if (renderer.sharedMaterials.Any(m => m != null && (m.shader == PlanarShadowEditorUtility.GetPlanarShadowBakedMat().shader || m.shader == PlanarShadowEditorUtility.GetPlanarShadowOriginalMat().shader)))
                    {
                        string message = $"그림자 머티리얼 포함: {renderer.gameObject.name} (경로: {path})";
                        Debug.LogWarning($"<color=yellow>[Planar Shadow] {message}</color>", renderer.gameObject);
                        foundShadows[renderer.gameObject] = message;
                    }
                }
            }

            private static void SelectObjectInHierarchyOrPrefab(GameObject obj, bool isPrefab)
            {
                if (isPrefab)
                {
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj);
                    if (prefab != null)
                    {
                        AssetDatabase.OpenAsset(prefab);
                        Selection.activeObject = prefab;
                    }
                }
                else
                {
                    Selection.activeGameObject = obj;
                    EditorGUIUtility.PingObject(obj);

                    if (SceneView.lastActiveSceneView != null)
                    {
                        SceneView.lastActiveSceneView.FrameSelected();
                    }
                }
            }

            public static void ShowWindow(Dictionary<GameObject, string> foundShadows)
            {
                _foundShadows = foundShadows;
                PlanarShadowCheckTool window = GetWindow<PlanarShadowCheckTool>("Planar Shadow Available List");
                window.Show();
            }

            private void OnGUI()
            {
                if (_foundShadows == null || _foundShadows.Count == 0)
                {
                    EditorGUILayout.LabelField("문제가 발견되지 않았습니다.");
                    return;
                }

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                foreach (var entry in _foundShadows)
                {
                    GameObject obj = entry.Key;
                    string message = entry.Value;
                    bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(obj);

                    EditorGUILayout.BeginHorizontal();
                    Texture icon = EditorGUIUtility.ObjectContent(obj, typeof(GameObject)).image;
                    GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));

                    if (GUILayout.Button(obj.name, EditorStyles.label, GUILayout.Width(200)))
                    {
                        SelectObjectInHierarchyOrPrefab(obj, isPrefab);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"- {message}", EditorStyles.miniLabel);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndScrollView();
            }
        }
        #endregion
    }
}
