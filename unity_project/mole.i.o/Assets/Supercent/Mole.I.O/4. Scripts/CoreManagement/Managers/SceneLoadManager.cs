using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    public class SceneLoadManager : MonoBehaviour
    {
        [SerializeField] EssentialPrefabs _essentialPrefabs;
        [SerializeField] CSVLoader _csvLoader;
        private void Awake()
        {
            GameManager.SetBakedDynamicData(_csvLoader.GetDynamicGameData().GetDataDic());
            GameManager.LoadEssentials();
            if (!GameManager.IsDynamicLoaded)
            {
                _csvLoader.StartLoadText();
            }

            if (GameManager.IsFirst())
            {
                Instantiate(_essentialPrefabs.NameUI);
            }

            if (!SceneLoadingUI.IsLoaded)
            {
                Instantiate(_essentialPrefabs.SceneLoadingUI);
            }

            Destroy(this);
        }
    }
}