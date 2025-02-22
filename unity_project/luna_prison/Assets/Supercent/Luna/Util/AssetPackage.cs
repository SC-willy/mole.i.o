using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Supercent.Util
{
    public sealed class AssetPackage : AssetPackageEditBase, IEnumerable<UnityObject>
    {
        readonly Dictionary<string, UnityObject> map = new Dictionary<string, UnityObject>();
        IDictionary<string, UnityObject> Map
        {
            get
            {
                if (!isInitialized)
                {
                    isInitialized = true;
                    Initialize();
                }
                return map;
            }
        }

#if UNITY_EDITOR
        [SerializeField] AssetTypes edit_types = default;
        protected override string edit_assetType => edit_types.GetSearchText();
#endif// UNITY_EDITOR

        [Space(10)]
        [SerializeField] List<UnityObject> assets = new List<UnityObject>();

        public int Count => assets.Count;
        public UnityObject this[int index] => assets[index];
        public IEnumerator GetEnumerator() => assets.GetEnumerator();
        IEnumerator<UnityObject> IEnumerable<UnityObject>.GetEnumerator() => assets.GetEnumerator();

        bool isInitialized = false;



        void Awake()
        {
            if (!isInitialized)
                Initialize();
        }
        void OnDestroy()
        {
            StopAllCoroutines();
            CancelInvoke();

            map.Clear();
            assets.Clear();
            Resources.UnloadUnusedAssets();
        }



        void Initialize()
        {
            map.Clear();
            for (int index = 0, cnt = assets.Count; index < cnt; ++index)
            {
                var asset = assets[index];
                if (asset == null) continue;
#if UNITY_EDITOR
                if (map.ContainsKey(asset.name))
                    Debug.LogError($"{nameof(AssetPackage)} : {asset.name} is an existing asset name.");
#endif// UNITY_EDITOR
                map[asset.name] = asset;
            }
        }

        public bool TryGetValue<T>(int index, out T result) where T : UnityObject
        {
            result = null;
            if (index < 0) return false;
            if (assets.Count <= index) return false;

            var asset = assets[index];
            if (asset is T castAsset)
            {
                result = castAsset;
                return true;
            }

            return false;
        }

        public bool TryGetValue<T>(string name, out T result) where T : UnityObject
        {
            if (Map.TryGetValue(name, out var obj)
             && obj is T asset)
            {
                result = asset;
                return true;
            }

            result = null;
            return false;
        }


#if UNITY_EDITOR
        protected override void Edit_OnLoadAssets(string[] guids)
        {
            foreach (var guid in guids)
            {
                var items = Edit_LoadAllAssetsAtPath(guid);
                if (items == null || items.Length < 1)
                    continue;

                var item = items[0];
                if (Edit_CheckCollect(item))
                {
                    if (item is Transform cast)
                        item = cast.gameObject;
                    assets.Add(item);
                }
            }
        }

        protected override void Edit_OnContextStart()
        {
            assets?.Clear();
            assets = new List<UnityObject>();
        }


        [Serializable]
        public struct AssetTypes
        {
            public string Manual;
            public bool AnimationClip;
            public bool AudioClip;
            public bool Font;
            public bool Material;
            public bool Mesh;
            public bool Model;
            public bool Object;
            public bool PhysicMaterial;
            public bool PhysicsMaterial2D;
            public bool Prefab;
            public bool RenderTexture;
            public bool ScriptableObject;
            public bool Shader;
            public bool Sprite;
            public bool SpriteAtlas;
            public bool TerrainLayer;
            public bool Texture;
            public bool Tile;
            public bool VideoClip;


            public string GetSearchText()
            {
                var result = string.Empty;
                {
                    if (AnimationClip) result += "t:AnimationClip,";
                    if (AudioClip) result += "t:AudioClip,";
                    if (Font) result += "t:Font,";
                    if (Material) result += "t:Material,";
                    if (Mesh) result += "t:Mesh,";
                    if (Model) result += "t:Model,";
                    if (Object) result += "t:Object,";
                    if (PhysicMaterial) result += "t:PhysicMaterial,";
                    if (PhysicsMaterial2D) result += "t:PhysicsMaterial2D,";
                    if (Prefab) result += "t:Prefab,";
                    if (RenderTexture) result += "t:RenderTexture,";
                    if (ScriptableObject) result += "t:ScriptableObject,";
                    if (Shader) result += "t:Shader,";
                    if (Sprite) result += "t:Sprite,";
                    if (SpriteAtlas) result += "t:SpriteAtlas,";
                    if (TerrainLayer) result += "t:TerrainLayer,";
                    if (Texture) result += "t:Texture,";
                    if (Tile) result += "t:Tile,";
                    if (VideoClip) result += "t:VideoClip,";
                    if (!string.IsNullOrEmpty(Manual)) result += Manual;
                }
                return result;
            }
        }
#endif// UNITY_EDITOR
    }
}