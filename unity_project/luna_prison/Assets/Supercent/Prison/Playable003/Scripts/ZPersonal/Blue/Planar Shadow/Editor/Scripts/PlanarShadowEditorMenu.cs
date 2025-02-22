using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Supercent.Rendering.Shadow.Editor
{
    public static class PlanarShadowEditorMenu
    {
        [MenuItem("Supercent/Planar Shadow/그림자 추가 및 제거/선택한 오브젝트 그림자 추가", false, 2)]
        private static void AddShadow()
        {
            if (false == EditorUtility.DisplayDialog("작업 수행 경고", "해당 작업은 수행 후 복구할 수 없습니다. 진행하십니까?", "네", "아니오"))
                return;

            int count = 0;

            foreach (GameObject go in Selection.gameObjects)
            {
                if (!go.TryGetComponent(out PlanarShadow _) && (go.TryGetComponent(out MeshRenderer _) || go.TryGetComponent(out SkinnedMeshRenderer _)))
                {
                    PlanarShadow planarShadow = go.AddComponent<PlanarShadow>();
                    planarShadow.Editor_FindComponents();
                    planarShadow.Editor_AddPlanarShadowMaterial();
                    ++count;
                }
            }

            Debug.Log($"<color=cyan>[Planar Shadow Editor] 총 {count}개의 오브젝트에 그림자가 추가되었습니다.</color>");

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("작업 완료", "작업이 완료되었습니다.", "확인");
        }

        [MenuItem("Supercent/Planar Shadow/그림자 추가 및 제거/선택한 오브젝트 그림자 제거", false, 3)]
        private static void RemoveShadow()
        {
            if (false == EditorUtility.DisplayDialog("작업 수행 경고", "해당 작업은 수행 후 복구할 수 없습니다. 진행하십니까?", "네", "아니오"))
                return;

            int count = 0;

            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.TryGetComponent(out PlanarShadow planarShadow))
                {
                    planarShadow.Editor_RemovePlanarShadowMaterial();
                    GameObject.DestroyImmediate(planarShadow, true);
                    ++count;
                }
            }

            Debug.Log($"<color=cyan>[Planar Shadow Editor] 총 {count}개의 오브젝트에서 그림자가 제거되었습니다.</color>");

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("작업 완료", "작업이 완료되었습니다.", "확인");
        }

        #region 모두 갱신
        [MenuItem("Supercent/Planar Shadow/그림자 안정화 도구/🚨 모든 그림자 (어셋 폴더 내 프리팹 및 모든 씬 오브젝트)", false, 4)]
        public static void ResetAllSettingsOnAnywhere()
        {
            if (false == EditorUtility.DisplayDialog("작업 수행 경고", "해당 작업은 수행 후 복구할 수 없습니다. 진행하십니까?", "네", "아니오"))
                return;

            RefreshPlanarShadowOnAssetsFolder();
            RefreshPlanarShadowOnAllScenes();

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("작업 완료", "작업이 완료되었습니다.", "확인");
        }

        [MenuItem("Supercent/Planar Shadow/그림자 안정화 도구/어셋 폴더 내 모든 프리팹", false, 5)]
        public static void ResetAllSettingsOnAssetsFolder()
        {
            if (false == EditorUtility.DisplayDialog("작업 수행 경고", "해당 작업은 수행 후 복구할 수 없습니다. 진행하십니까?", "네", "아니오"))
                return;

            RefreshPlanarShadowOnAssetsFolder();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("작업 완료", "작업이 완료되었습니다.", "확인");
        }

        [MenuItem("Supercent/Planar Shadow/그림자 안정화 도구/현재 씬 내 모든 오브젝트", false, 6)]
        private static void ResetAllSettingsOnCurrentScene()
        {
            if (false == EditorUtility.DisplayDialog("작업 수행 경고", "해당 작업은 수행 후 복구할 수 없습니다. 진행하십니까?", "네", "아니오"))
                return;            

            RefreshPlanarShadowOnCurrentScene();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("작업 완료", "작업이 완료되었습니다.", "확인");
        }

        public static void RefreshPlanarShadowOnAssetsFolder()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            int total = prefabGuids.Length;
            int processed = 0;

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

                    RefreshPlanarShadowInGameObject(asset, true, path);
                    processed++;
                    EditorUtility.DisplayProgressBar("Planar Shadow 갱신", $"어셋 폴더 프리팹 처리 중... ({processed}/{total})", (float)processed / total);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void RefreshPlanarShadowOnCurrentScene()
        {
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
                        RefreshPlanarShadowInGameObject(rootObject, false, currentScenePath);
                        processedObjects++;
                        EditorUtility.DisplayProgressBar("Planar Shadow 갱신", $"현재 씬 오브젝트 처리 중... ({processedObjects}/{totalObjects})", (float)processedObjects / totalObjects);
                    }

                    EditorSceneManager.SaveScene(currentScene);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void RefreshPlanarShadowOnAllScenes()
        {
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
                }

                string[] allScenePaths = AssetDatabase.FindAssets("t:Scene")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .ToArray();

                int totalScenes = allScenePaths.Length;
                int processedScenes = 0;

                foreach (string scenePath in allScenePaths)
                {
                    Scene loadedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    GameObject[] rootGameObjects = loadedScene.GetRootGameObjects();
                    int totalObjects = rootGameObjects.Length;
                    int processedObjects = 0;

                    foreach (var rootObject in rootGameObjects)
                    {
                        RefreshPlanarShadowInGameObject(rootObject, false, scenePath);
                        processedObjects++;
                        EditorUtility.DisplayProgressBar("Planar Shadow 갱신", $"씬: {scenePath} 처리 중... ({processedObjects}/{totalObjects})", (float)processedObjects / totalObjects);
                    }

                    EditorSceneManager.SaveScene(loadedScene);
                    processedScenes++;
                    EditorUtility.DisplayProgressBar("Planar Shadow 갱신", $"전체 씬 처리 중... ({processedScenes}/{totalScenes})", 0.5f + ((float)processedScenes / totalScenes) * 0.5f);
                }

                if (!string.IsNullOrEmpty(currentScenePath))
                {
                    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void RefreshPlanarShadowInGameObject(GameObject target, bool isPrefab, string assetPath)
        {
            PlanarShadow[] planarShadows = target.GetComponentsInChildren<PlanarShadow>(true);
            bool isModified = false;

            foreach (var planarShadow in planarShadows)
            {
                if (!PrefabUtility.IsPartOfPrefabInstance(planarShadow.gameObject) ||
                    PrefabUtility.HasPrefabInstanceAnyOverrides(planarShadow.gameObject, false))
                {
                    planarShadow.Editor_ResetSettings();
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
        #endregion

        #region 선택 갱신
        [MenuItem("Supercent/Planar Shadow/그림자 안정화 도구/선택한 오브젝트", false, 7)]
        private static void ResetSettingsOnSelection()
        {
            if (false == EditorUtility.DisplayDialog("작업 수행 경고", "해당 작업은 수행 후 복구할 수 없습니다. 진행하십니까?", "네", "아니오"))
                return;

            List<PlanarShadow> planarShadows = Selection.gameObjects
                                        .Where(x => x.GetComponent<PlanarShadow>() != null)
                                        .Select(x => x.GetComponent<PlanarShadow>())
                                        .ToList();

            foreach (PlanarShadow planarShadow in planarShadows)
            {
                planarShadow.Editor_ResetSettings();
                EditorUtility.SetDirty(planarShadow);
            }

            EditorUtility.DisplayDialog("작업 완료", "작업이 완료되었습니다.", "확인");
        }
        #endregion

        #region 기능
        #endregion

        #region 피벗 그림자 오프셋 미설정 비활성
        [MenuItem("Supercent/Planar Shadow/기타/🚨 설정되지 않은 피벗 그림자 비활성 (어셋 폴더 내 프리팹 및 모든 씬 오브젝트)", false, 13)]
        private static void CleanNonDefinedPivotRenderSettings()
        {
            if (false == EditorUtility.DisplayDialog("작업 수행 경고", "해당 작업은 수행 후 복구할 수 없습니다. 진행하십니까?", "네", "아니오"))
                return;

            CleanNonDefinedPivotRenderSettingsOnAssetsFolder();
            CleanNonDefinedPivotRenderSettingsOnScenes();

            EditorUtility.DisplayDialog("작업 완료", "작업이 완료되었습니다.", "확인");
        }

        private static void CleanNonDefinedPivotRenderSettingsOnAssetsFolder()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            int total = prefabGuids.Length;
            int processed = 0;

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

                    CleanNonDefinedPivotRenderSetting(asset, true, path);
                    processed++;
                    EditorUtility.DisplayProgressBar("그림자 제거", $"에셋 처리 중... ({processed}/{total})", (float)processed / total);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void CleanNonDefinedPivotRenderSettingsOnScenes()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            string currentScenePath = currentScene.path;
            bool isCurrentSceneDirty = currentScene.isDirty;

            try
            {
                if (!string.IsNullOrEmpty(currentScenePath) && isCurrentSceneDirty)
                {
                    if (EditorUtility.DisplayDialog("씬 변경 사항 저장", "현재 열린 씬에 변경 사항이 있습니다. 저장하시겠습니까?", "저장", "취소"))
                    {
                        EditorSceneManager.SaveScene(currentScene);
                    }
                }

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
                        CleanNonDefinedPivotRenderSetting(rootObject, false, scenePath);
                    }

                    EditorSceneManager.SaveScene(loadedScene);
                    processed++;
                    EditorUtility.DisplayProgressBar("그림자 제거", $"씬 처리 중... ({processed}/{total})", 0.5f + (float)processed / total * 0.5f);
                }

                if (!string.IsNullOrEmpty(currentScenePath))
                {
                    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void CleanNonDefinedPivotRenderSetting(GameObject target, bool isPrefab, string assetPath)
        {
            PlanarShadow[] planarShadows = target.GetComponentsInChildren<PlanarShadow>(true);
            bool isModified = false;

            foreach (var planarShadow in planarShadows)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(planarShadow.gameObject))
                {
                    if (PrefabUtility.HasPrefabInstanceAnyOverrides(planarShadow.gameObject, false))
                    {
                        if (planarShadow.Editor_UsePivotShadow)
                        {
                            if (false == planarShadow.Editor_IsPivotShadowOffsetDefined)
                            {
                                planarShadow.Editor_RefreshMaterialArray();
                                Debug.Log($"<color=yellow>[Planar Shadow] {planarShadow.gameObject.name} - 프리팹 인스턴스에서 피벗 설정 비활성</color>");
                                planarShadow.Editor_TogglePivotRenderingMode(false);
                                EditorUtility.SetDirty(planarShadow);
                                isModified = true;
                            }
                        }
                    }
                    continue;
                }

                if (planarShadow.Editor_UsePivotShadow)
                {
                    if (false == planarShadow.Editor_IsPivotShadowOffsetDefined)
                    {
                        planarShadow.Editor_RefreshMaterialArray();
                        Debug.Log($"<color=yellow>[Planar Shadow] {planarShadow.gameObject.name} - 피벗 설정 비활성</color>");
                        planarShadow.Editor_TogglePivotRenderingMode(false);
                        EditorUtility.SetDirty(planarShadow);
                        isModified = true;
                    }
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
        #endregion
    }
}
