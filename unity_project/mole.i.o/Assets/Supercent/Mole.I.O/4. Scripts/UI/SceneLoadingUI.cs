using System.Collections;
using Supercent.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Supercent.MoleIO.Management
{
    public class SceneLoadingUI : MonoBehaviour
    {
        const string LOAD_SCENE = "Loader";
        const int STAGE_LENGTH = 1;

        public static bool IsLoaded => _isLoaded;
        static bool _isLoaded = false;

        [SerializeField] float _loadDelay;
        [SerializeField] GameObject _loadingScreen; // 로딩 UI
        [SerializeField] Slider _progressBar; // 진행도 바 (UI)

        void Awake()
        {
            _isLoaded = true;
            DontDestroyOnLoad(this);
            GameManager.GetLoadUI(this);
        }
        public void LoadScene()
        {
            if (STAGE_LENGTH <= PlayerData.Stage)
                StartCoroutine(LoadSceneAsync($"Stage00{STAGE_LENGTH - 1}"));
            else
                StartCoroutine(LoadSceneAsync($"Stage00{PlayerData.Stage}"));
        }

        public void BackToLoader()
        {
            StartCoroutine(LoadSceneAsync(LOAD_SCENE));
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            _loadingScreen.SetActive(true);
            yield return CoroutineUtil.WaitForSeconds(_loadDelay);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                _progressBar.value = progress;

                if (operation.progress >= 0.9f)
                {
                    yield return CoroutineUtil.WaitForSeconds(0.1f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            _loadingScreen.SetActive(false);
        }
    }

}
