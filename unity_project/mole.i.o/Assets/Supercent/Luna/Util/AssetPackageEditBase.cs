using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Supercent.Util
{
    public abstract class AssetPackageEditBase : MonoBehaviour
    {
#if UNITY_EDITOR
        const string EngineName = "UnityEngine";

        enum SearchType
        {
            Free = 0,
            Prefix,
            Suffix
        }

        [SerializeField] UnityEditor.DefaultAsset edit_folder = null;
        [SerializeField] bool edit_isIncludeChildFolders = false;

        [Space(10)]
        [SerializeField] SearchType edit_searchType = SearchType.Prefix;
        [SerializeField] StringComparison edit_searchComparison = StringComparison.OrdinalIgnoreCase;
        [SerializeField] string edit_searchText = string.Empty;

        protected abstract string edit_assetType { get; }
        int edit_folderDepth = 0;



        protected abstract void Edit_OnLoadAssets(string[] guids);

        protected UnityObject[] Edit_LoadAllAssetsAtPath(string guid)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) return null;

            var depth = Edit_GetFolderDepth(path);
            if (depth < edit_folderDepth) return null;
            if (!edit_isIncludeChildFolders && edit_folderDepth < depth) return null;

            return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
        }

        protected UnityObject[] Edit_LoadAllAssetRepresentationsAtPath(string guid)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) return null;

            var depth = Edit_GetFolderDepth(path);
            if (depth < edit_folderDepth) return null;
            if (!edit_isIncludeChildFolders && edit_folderDepth < depth) return null;

            return UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        }

        protected bool Edit_CheckCollect(UnityObject obj)
        {
            var objNamespace = obj?.GetType()?.Namespace;
            if (string.IsNullOrEmpty(objNamespace)) return false;
            if (objNamespace.Length < EngineName.Length) return false;
            if (objNamespace.IndexOf(EngineName, 0, EngineName.Length) != 0) return false;

            var name = obj.name;
            if (!string.IsNullOrEmpty(edit_searchText))
            {
                var lenSearch = edit_searchText.Length;
                if (name.Length < lenSearch)
                    return false;

                switch (edit_searchType)
                {
                case SearchType.Prefix: return name.IndexOf(edit_searchText, 0, lenSearch, edit_searchComparison) == 0;
                case SearchType.Suffix: return name.LastIndexOf(edit_searchText, name.Length - lenSearch, lenSearch, edit_searchComparison) == name.Length - lenSearch;
                default: return -1 < name.IndexOf(edit_searchText, edit_searchComparison);
                }
            }
            return true;
        }

        int Edit_GetFolderDepth(string path) => Regex.Matches(path, "/").Count;


        protected abstract void Edit_OnContextStart();

        [UnityEditor.MenuItem("CONTEXT/AssetPackageEditBase/Collect Assets", priority = 1000)]
        static void Edit_ExportKeys(UnityEditor.MenuCommand menuCommand)
        {
            if (!UnityEditor.EditorApplication.isPlaying
             && menuCommand.context is AssetPackageEditBase comp)
            {
                comp.Edit_OnContextStart();

                var path = comp.edit_folder == null
                         ? "Assets"
                         : UnityEditor.AssetDatabase.GetAssetPath(comp.edit_folder);

                if (string.IsNullOrEmpty(path))
                {
                    UnityEditor.EditorUtility.DisplayDialog("Info", "Invalid path.", "ok");
                    return;
                }

                comp.edit_folderDepth = comp.Edit_GetFolderDepth(path) + 1;
                var guids = UnityEditor.AssetDatabase.FindAssets(comp.edit_assetType, new string[] { path });
                if (0 < guids?.Length)
                    comp.Edit_OnLoadAssets(guids);

                UnityEditor.EditorUtility.SetDirty(comp);
            }
        }
#endif// UNITY_EDITOR;
    }
}