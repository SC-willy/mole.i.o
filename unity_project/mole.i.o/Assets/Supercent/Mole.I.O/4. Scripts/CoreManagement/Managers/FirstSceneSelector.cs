using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    public class FirstSceneSelector : MonoBehaviour
    {
        [SerializeField] EssentialPrefabs _essentialPrefabs;
        private void Awake()
        {
            GameManager.LoadEssentials();

            if (!SceneLoadingUI.IsLoaded)
                Instantiate(_essentialPrefabs.SceneLoadingUI);

            GameManager.LoadLoadScene();
        }
    }
}