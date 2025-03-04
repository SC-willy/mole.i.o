using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "EssentialPrefabs", menuName = "Manage/EssentialPrefabs")]
    public class EssentialPrefabs : ScriptableObject
    {
        [SerializeField] GameObject _nameUI;
        public GameObject NameUI => _nameUI;
        [SerializeField] GameObject _sceneLoadingUI;
        public GameObject SceneLoadingUI => _sceneLoadingUI;
    }
}