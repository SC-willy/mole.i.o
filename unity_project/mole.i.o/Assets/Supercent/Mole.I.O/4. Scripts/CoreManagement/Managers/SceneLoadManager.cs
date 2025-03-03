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
            if (!GameManager.IsLoaded())
            {
                _csvLoader.StartLoadText();
            }

            if (GameManager.IsFirst())
            {
                Instantiate(_essentialPrefabs.NameUI);
            }

            Destroy(this);
        }
    }
}