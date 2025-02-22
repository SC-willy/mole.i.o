using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Supercent.Rendering.Shadow.Editor.Utility
{
    [InitializeOnLoad]
    public static class PlanarShadowVersionChecker
    {
        private const string URL = "https://raw.githubusercontent.com/Bonnate/VersionChecker/main/planarshadow.json";
        private const string CURRENT_VERSION = "1.0.1";
        private const string UPDATE_LINK = "https://www.notion.so/supercent/10a93b2d25738022a4b6f6edf615781c?pvs=4";
        private const string PREFS_KEY = "PlanarShadowVersionCheckDone";
        private static string _checkedVersion = null;

        static PlanarShadowVersionChecker()
        {
            EditorApplication.quitting += ResetVersionCheckFlag;

            if (!EditorPrefs.GetBool(PREFS_KEY, false))
            {
                CheckVersion();
            }
        }

        [MenuItem("Supercent/Planar Shadow/업데이트 확인", false, int.MaxValue)]
        private static void CheckVersion()
        {
            EditorApplication.delayCall += async () =>
            {
                await RunVersionCheckAsync();
                EditorPrefs.SetBool(PREFS_KEY, true);
            };
        }

        private static async Task RunVersionCheckAsync()
        {
            PlanarShadowVersionData versionData = await FetchVersionFromJsonAsync();
            if (versionData == null || string.IsNullOrEmpty(versionData.PlanarShadow))
            {
                Debug.LogWarning("<color=yellow>[Planar Shadow] 버전 정보를 가져오는데 실패했습니다.</color>");
                return;
            }

            _checkedVersion = versionData.PlanarShadow;
            if (_checkedVersion == CURRENT_VERSION)
            {
                Debug.Log($"<color=cyan>[Planar Shadow] 최신 버전({_checkedVersion})을 사용하고 있습니다.</color>");
            }
            else
            {
                ShowUpdateDialog();
            }
        }

        private static async Task<PlanarShadowVersionData> FetchVersionFromJsonAsync()
        {
            using HttpClient client = new HttpClient();
            try
            {
                string cacheBypassUrl = $"{URL}?t={DateTime.UtcNow.Ticks}";
                string response = await client.GetStringAsync(cacheBypassUrl);

                return JsonUtility.FromJson<PlanarShadowVersionData>(response);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<color=yellow>[Planar Shadow] JSON 데이터 가져오는 중 오류 발생: {ex.Message}</color>");
                return null;
            }
        }

        private static void ShowUpdateDialog()
        {
            EditorApplication.delayCall += () =>
            {   
                string message = $"최신 버전을 이용하고 있지 않습니다. 현재 버전은 {CURRENT_VERSION}입니다. 최신 버전은 {_checkedVersion}입니다.";

                Debug.Log($"<color=yellow>[Planar Shadow] {message}</color>");

                if (EditorUtility.DisplayDialog(
                    "Planar Shadow 버전 검사",
                    message,
                    "확인",
                    "노션 페이지 열기"))
                {
                }
                else
                {
                    Application.OpenURL(UPDATE_LINK);
                }
            };
        }

        private static void ResetVersionCheckFlag()
        {
            EditorPrefs.DeleteKey(PREFS_KEY);
        }
    }

    [Serializable]
    public class PlanarShadowVersionData
    {
        public string PlanarShadow;
    }
}
