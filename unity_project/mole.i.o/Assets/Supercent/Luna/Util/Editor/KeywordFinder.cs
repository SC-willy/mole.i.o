using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Supercent.Util.Editor
{
    public class KeywordFinder : EditorWindowBase
    {
        const string TAG = nameof(KeywordFinder);

        const string RootPath = "Assets";
        const string BasePath = "Supercent";

        [Serializable]
        public class Preset
        {
            public string[] keywords;
        }

        struct PathData
        {
            public string Name;
            public string Path;
            public List<string> Keywords;
        }

        readonly List<PathData> assets = new List<PathData>();
        readonly string[] paths = { string.IsNullOrEmpty(BasePath) ? RootPath : Path.Combine(RootPath, BasePath) };
        readonly static List<string> tempPath = new List<string>();
        string lastPath = BasePath;
        bool foldScriptList = true;
        Vector2 scrollPos = Vector2.zero;

        [SerializeField] string[] keywords = null;
        SerializedProperty propKeywords = null;

        GUIStyle stylePathLable = null;
        GUIStyle stylePathText = null;
        GUIStyle styleMenuButton = null;
        GUIStyle styleAssetButton = null;



        [MenuItem("Supercent/Util/Keyword finder")]
        public static void OpenWindow()
        {
            GetWindow<KeywordFinder>("Keyword finder");
        }

        void OnEnable()
        {
            stylePathLable = null;
            stylePathText = null;
            styleMenuButton = null;
            styleAssetButton = null;

            propKeywords = THIS.FindProperty(nameof(keywords));

            guiDraw = GUIDraw;
        }



        #region Draw menu
        void GUIDraw()
        {
            SetStyle();

            THIS.Update();
            ViewTopMenu();
            EditorGUILayout.Space();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none);
            {
                EditorGUILayout.PropertyField(propKeywords);
                DivisionLine(Color.gray, 5, 1f);
                ViewScriptList();
            }
            EditorGUILayout.EndScrollView();
            THIS.ApplyModifiedProperties();
        }

        void SetStyle()
        {
            if (stylePathLable == null)
            {
                stylePathLable = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset { top = 3 },
                };
            }
            if (stylePathText == null)
            {
                stylePathText = new GUIStyle(GUI.skin.textField)
                {
                    alignment = TextAnchor.MiddleLeft,
                    margin = new RectOffset { top = 3, right = 3, left = 3, },
                    fixedHeight = 18,
                };
            }
            if (styleMenuButton == null)
            {
                styleMenuButton = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset { left = 2, bottom = 1 },
                    fontSize = 12,
                    fixedWidth = 18,
                    fixedHeight = 18,
                };
                var margin = styleMenuButton.margin;
                margin.top += 1;
                styleMenuButton.margin = margin;
            }

            if (styleAssetButton == null)
            {
                styleAssetButton = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    clipping = TextClipping.Clip,
                    padding = new RectOffset { bottom = 1 },
                    richText = true,
                    fixedHeight = 50,
                    stretchWidth = true,
                };
            }
        }

        void ViewTopMenu()
        {
            EditorGUILayout.BeginHorizontal();
            {
                var editPath = EditorGUILayout.TextField(lastPath, stylePathText);
                if (string.CompareOrdinal(editPath, lastPath) != 0)
                    SetPath(editPath);

                if (Button("O", styleMenuButton))
                {
                    if (OpenAssetsFolderPanel("Select a folder", lastPath, string.Empty, out var assetsPath))
                        SetPath(assetsPath);
                }

                if (Button("D", styleMenuButton))
                    SetPath(BasePath);

                if (Button("C", styleMenuButton))
                    SetPath(string.Empty);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            {
                if (Button("Find"))
                    FindAll();
                if (Button("Reset"))
                    assets.Clear();
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            {
                if (Button("Save"))
                {
                    var pathSave = EditorUtility.SaveFilePanel("Save preset", PathProjectSetting, "FindKeywords", "preset");
                    if (!string.IsNullOrEmpty(pathSave))
                        SavePreset(pathSave);
                }

                if (Button("Load"))
                {
                    var pathLoad = EditorUtility.OpenFilePanel("Load preset", PathProjectSetting, "preset");
                    if (!string.IsNullOrEmpty(pathLoad))
                        LoadPreset(pathLoad);
                }

                if (Button("Clear"))
                    keywords = null;
            }
            EditorGUILayout.EndHorizontal();
        }

        void ViewScriptList()
        {
            if (foldScriptList = EditorGUILayout.Foldout(foldScriptList, "Scripts"))
            {
                var sizeBase = styleAssetButton.fontSize;
                var sizeName = sizeBase + 4;

                for (int index = 0; index < assets.Count; ++index)
                {
                    var asset = assets[index];
                    var strName = $"<size={sizeName}><color=#FF9999><b>{asset.Name}</b></Color></size>\n({string.Join(", ", asset.Keywords)})";

                    if (Button(strName, styleAssetButton))
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(asset.Path);
                }
            }
        }
        #endregion// Draw menu


        #region Find
        void SetPath(string path)
        {
            lastPath = path;
            paths[0] = string.IsNullOrEmpty(lastPath)
                     ? RootPath
                     : Path.Combine(RootPath, lastPath);
        }

        void FindAll()
        {
            assets.Clear();
            if (keywords == null || keywords.Length < 1)
                return;

            keywords = keywords.Distinct().ToArray();

            tempPath.Clear();
            {
                var fileGUIDs = AssetDatabase.FindAssets("t:Script", paths);

                for (int index = 0; index < fileGUIDs.Length; ++index)
                {
                    var path = AssetDatabase.GUIDToAssetPath(fileGUIDs[index]);
                    if (string.IsNullOrEmpty(path)) continue;
                    if (-1 < path.IndexOf("editor", StringComparison.OrdinalIgnoreCase)) continue;

                    tempPath.Add(path);
                }

                Parallel.ForEach(tempPath, ScriptCheck);
                assets.Sort((a, b) => string.CompareOrdinal(a.Path, b.Path));
            }
            tempPath.Clear();
        }

        void ScriptCheck(string path)
        {
            if (!File.Exists(path))
                return;

            var script = File.ReadAllText(path);
            if (string.IsNullOrEmpty(script))
                return;

            var keywords = new List<string>();
            for (int index = 0; index < this.keywords.Length; ++index)
            {
                var str = this.keywords[index];
                if (-1 < script.IndexOf(str, StringComparison.OrdinalIgnoreCase))
                    keywords.Add(str);
            }

            if (keywords.Count < 1)
                return;

            var offset = path.LastIndexOf('/');
            var asset = new PathData()
            {
                Name = offset < 0 ? path : path.Substring(offset + 1),
                Path = path,
                Keywords = keywords,
            };

            lock ((assets as ICollection).SyncRoot)
                assets.Add(asset);
        }
        #endregion// Find


        #region Info IO
        void SavePreset(string path)
        {
            var preset = new Preset()
            {
                keywords = keywords,
            };

            SaveJson(path, preset);
        }

        void LoadPreset(string path)
        {
            // Load file
            if (!LoadJson(path, out Preset preset) || preset == null)
            {
                EditorUtility.DisplayDialog("Error", "Invalid preset", "Ok");
                return;
            }

            keywords = preset.keywords;
        }
        #endregion// Info IO
    }
}