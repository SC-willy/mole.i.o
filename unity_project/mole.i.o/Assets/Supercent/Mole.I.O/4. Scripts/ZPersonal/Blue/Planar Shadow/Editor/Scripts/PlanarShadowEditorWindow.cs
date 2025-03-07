using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

namespace Supercent.Rendering.Shadow.Editor
{
    public class PlanarShadowEditorWindow : EditorWindow
    {
        private enum EShowOnlyTypes
        {
            All,
            AddShadowOnly,
            RemoveShadowOnly,
        }

        private const string SHOW_INACTIVE_KEY = "PLANAR_SHADOW_EDITOR_SHOW_INACTIVE";
        private const string SHOW_ONLY_TYPE_KEY = "PLANAR_SHADOW_EDITOR_SHOW_ONLY_TYPE";
        private List<GameObject> _meshRendererObjects = new List<GameObject>();
        private List<GameObject> _skinnedMeshRendererObjects = new List<GameObject>();
        private Vector2 _scrollPosition;
        private string _searchQuery = "";
        private bool _alwaysOnTop = false;
        private static bool _showInactive = false;
        private static EShowOnlyTypes _showOnlyType = EShowOnlyTypes.All;
        private readonly string[] _showOnlyTypeOptions = new string[] { "모두 표시", "그림자 추가만 표시", "그림자 제거만 표시" };
        private Color _shadowColor;
        private Color originalShadowColor;
        private Vector3 _shadowDirection;
        private Vector3 originalShadowDirection;
        private float _shadowHeight;
        private float originalShadowHeight;
        private Material _editor_planar_shadow_mat;
        private Material _editor_planar_shadow_baked_mat;
        private static EditorWindow _window = null;

        [MenuItem("Supercent/Planar Shadow/에디터 창 열기", false, 1)]
        public static void ShowWindow()
        {
            _window = GetWindow<PlanarShadowEditorWindow>("Planar Shadow Editor");
            _window.Show();
        }

        private void OnEnable()
        {
            _showInactive = EditorPrefs.GetBool(SHOW_INACTIVE_KEY, false);
            _showOnlyType = (EShowOnlyTypes)EditorPrefs.GetInt(SHOW_ONLY_TYPE_KEY, (int)EShowOnlyTypes.All);

            FindObjectsWithRenderers();

            _editor_planar_shadow_mat = Resources.Load<Material>("PlanarShadowMat");
            _editor_planar_shadow_baked_mat = Resources.Load<Material>("PlanarShadowBakedMat");

            if (_editor_planar_shadow_mat != null)
            {
                _shadowColor = _editor_planar_shadow_mat.GetColor("_ShadowColor");
                originalShadowColor = _shadowColor;

                _shadowDirection = _editor_planar_shadow_mat.GetVector("_LightDirection");
                originalShadowDirection = _shadowDirection;

                _shadowHeight = _editor_planar_shadow_mat.GetFloat("_PlaneHeight");
                originalShadowHeight = _shadowHeight;
            }
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GameObject[] selections = Selection.gameObjects;

            GUILayout.BeginHorizontal("box");
            GUILayout.Label(GetIcon("GameObject On Icon"), GUILayout.Width(15), GUILayout.Height(15));
            GUILayout.Label("선택한 오브젝트 제어");
            GUILayout.EndHorizontal();

            if (selections.Length > 0)
            {
                int totalSelected = selections.Length;
                int planarShadowCount = 0;
                int addablePlanarShadowCount = 0;

                foreach (GameObject go in selections)
                {
                    if (go.TryGetComponent<PlanarShadow>(out _))
                        planarShadowCount++;
                    else if (go.TryGetComponent<Renderer>(out _))
                        addablePlanarShadowCount++;
                }

                EditorGUILayout.HelpBox(
                    $"'{SceneManager.GetActiveScene().name}' 씬에서 총 {totalSelected}개의 오브젝트가 선택되었습니다. 그림자를 추가하거나 제거하는 작업을 간편하게 할 수 있습니다.",
                    MessageType.Info);

                GUILayout.BeginHorizontal();

                GUI.enabled = planarShadowCount > 0;
                if (GUILayout.Button("그림자 모두 제거", GUILayout.Width(150)))
                {
                    foreach (GameObject go in selections)
                    {
                        if (go.TryGetComponent<PlanarShadow>(out PlanarShadow planarShadow))
                        {
                            planarShadow.Editor_RemovePlanarShadowMaterial();
                            DestroyImmediate(planarShadow);
                        }
                    }
                }
                GUI.enabled = true;

                GUILayout.Space(10);
                EditorGUILayout.LabelField($"그림자가 이미 추가된 오브젝트 개수: {planarShadowCount}", EditorStyles.label);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUI.enabled = addablePlanarShadowCount > 0;
                if (GUILayout.Button("그림자 모두 추가", GUILayout.Width(150)))
                {
                    foreach (GameObject go in selections)
                    {
                        if (go.TryGetComponent<Renderer>(out _) && !go.TryGetComponent<PlanarShadow>(out _))
                        {
                            PlanarShadow planarShadow = go.AddComponent<PlanarShadow>();
                            planarShadow.Editor_FindComponents();
                            planarShadow.Editor_AddPlanarShadowMaterial();
                        }
                    }
                }
                GUI.enabled = true;

                GUILayout.Space(10);
                EditorGUILayout.LabelField($"그림자가 추가될 수 있는 오브젝트 개수: {addablePlanarShadowCount}", EditorStyles.label);
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"현재 선택된 오브젝트가 없습니다. '{SceneManager.GetActiveScene().name}' 씬에서 오브젝트를 선택하면 이곳에 나타납니다.",
                    MessageType.Info);
            }

            GUILayout.Space(20);

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(GetIcon("d_Material Icon"), GUILayout.Width(15), GUILayout.Height(15));
            GUILayout.Label("그림자 머티리얼 속성");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.Label("그림자 색상", GUILayout.Width(150));
            _shadowColor = EditorGUILayout.ColorField(_shadowColor);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            GUILayout.Label("그림자 방향", GUILayout.Width(150));

            _shadowDirection.x = EditorGUILayout.FloatField("X", _shadowDirection.x, GUILayout.Width(300));
            _shadowDirection.y = EditorGUILayout.FloatField("Y", _shadowDirection.y, GUILayout.Width(300));
            _shadowDirection.z = EditorGUILayout.FloatField("Z", _shadowDirection.z, GUILayout.Width(300));

            GUILayout.Space(30);

            if (GUILayout.Button("그림자 방향 자동 설정", GUILayout.Width(200)))
            {
                foreach (Light light in FindObjectsOfType<Light>())
                {
                    if (light.type == LightType.Directional)
                    {
                        _shadowDirection = light.transform.forward;
                    }
                }
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Label("그림자 표시 높이", GUILayout.Width(150));
            _shadowHeight = EditorGUILayout.FloatField("Y", _shadowHeight);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(10);

            if (EditorGUI.EndChangeCheck()) 
            {
                ApplyMaterialChanges(); 
            }

            if (IsMaterialChanged())
            {
                if (GUILayout.Button("변경 사항 복구"))
                {
                    DiscardMaterialChanges();
                }
            }


            GUILayout.Space(10);

            GUILayout.BeginHorizontal("box");

            string new_searchQuery = GUILayout.TextField(_searchQuery, GUILayout.Width(200));
            if (new_searchQuery != _searchQuery)
            {
                _searchQuery = new_searchQuery;
                FindObjectsWithRenderers();
            }

            if (GUILayout.Button("검색", GUILayout.Width(100)))
            {
                FindObjectsWithRenderers();
            }

            GUILayout.Space(30);

            _alwaysOnTop = GUILayout.Toggle(_alwaysOnTop, "항상 위에 표시", GUILayout.Width(100));
            if (_alwaysOnTop)
                _window.Focus();

            GUILayout.Space(10);

            bool new_showInactive = GUILayout.Toggle(_showInactive, "비활성 오브젝트 표시", GUILayout.Width(140));
            if (new_showInactive != _showInactive)
            {
                _showInactive = new_showInactive;
                EditorPrefs.SetBool(SHOW_INACTIVE_KEY, _showInactive);
            }

            GUILayout.Space(10);

            int selectedShowOnlyTypeIndex = (int)_showOnlyType;

            selectedShowOnlyTypeIndex = EditorGUILayout.Popup(selectedShowOnlyTypeIndex, _showOnlyTypeOptions, GUILayout.Width(160));

            if (selectedShowOnlyTypeIndex != (int)_showOnlyType)
            {
                _showOnlyType = (EShowOnlyTypes)selectedShowOnlyTypeIndex;
                EditorPrefs.SetInt(SHOW_ONLY_TYPE_KEY, selectedShowOnlyTypeIndex);
                FindObjectsWithRenderers();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label(GetIcon("d_MeshRenderer Icon"), GUILayout.Width(15), GUILayout.Height(15));
            GUILayout.Label(" Mesh Renderers", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            DisplayObjects(_meshRendererObjects);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal("box");
            GUILayout.Label(GetIcon("d_SkinnedMeshRenderer Icon"), GUILayout.Width(15), GUILayout.Height(15));
            GUILayout.Label("Skinned Mesh Renderers", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            DisplayObjects(_skinnedMeshRendererObjects);

            EditorGUILayout.EndScrollView();
        }

        private void ApplyMaterialChanges()
        {
            if (_editor_planar_shadow_mat != null)
            {
                _editor_planar_shadow_mat.SetColor("_ShadowColor", _shadowColor);
                _editor_planar_shadow_baked_mat.SetColor("_ShadowColor", _shadowColor);
                _editor_planar_shadow_mat.SetVector("_LightDirection", _shadowDirection);
                _editor_planar_shadow_mat.SetFloat("_PlaneHeight", _shadowHeight);

                EditorUtility.SetDirty(_editor_planar_shadow_mat);
            }
            else
            {
                Debug.LogError("<color=red>[Planar Shadow] 그림자 머티리얼을 찾을 수 없습니다!</color>");
            }
        }

        private void DiscardMaterialChanges()
        {
            _shadowColor = originalShadowColor;
            _shadowDirection = originalShadowDirection;
            _shadowHeight = originalShadowHeight;

            ApplyMaterialChanges();
        }

        private bool IsMaterialChanged()
        {
            return _shadowColor != originalShadowColor ||
                   _shadowDirection != originalShadowDirection ||
                   _shadowHeight != originalShadowHeight;
        }

        private void FindObjectsWithRenderers()
        {
            _meshRendererObjects.Clear();
            _skinnedMeshRendererObjects.Clear();

            MeshRenderer[] meshRenderers = FindObjectsOfType<MeshRenderer>(true).Where(mR => mR.GetComponent<TMP_Text>() == null).ToArray();
            SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>(true).Where(mR => mR.GetComponent<TMP_Text>() == null).ToArray();

            foreach (var renderer in meshRenderers)
            {
                if (FilterByShowOnlyType(renderer.gameObject))
                {
                    _meshRendererObjects.Add(renderer.gameObject);
                }
            }

            foreach (var renderer in skinnedMeshRenderers)
            {
                if (FilterByShowOnlyType(renderer.gameObject))
                {
                    _skinnedMeshRendererObjects.Add(renderer.gameObject);
                }
            }

            _meshRendererObjects = _meshRendererObjects.OrderBy(obj => obj.name).ToList();
            _skinnedMeshRendererObjects = _skinnedMeshRendererObjects.OrderBy(obj => obj.name).ToList();

            if (!string.IsNullOrEmpty(_searchQuery))
            {
                _meshRendererObjects = _meshRendererObjects.Where(obj => obj.name.ToLower().Contains(_searchQuery.ToLower())).ToList();
                _skinnedMeshRendererObjects = _skinnedMeshRendererObjects.Where(obj => obj.name.ToLower().Contains(_searchQuery.ToLower())).ToList();
            }
        }

        private static bool FilterByShowOnlyType(GameObject obj)
        {
            switch (_showOnlyType)
            {
                case EShowOnlyTypes.AddShadowOnly:
                    return obj.GetComponent<Renderer>() != null && obj.GetComponent<PlanarShadow>() == null;
                case EShowOnlyTypes.RemoveShadowOnly:
                    return obj.GetComponent<PlanarShadow>() != null;
                default:
                    return true;
            }
        }

        private void DisplayObjects(List<GameObject> objects)
        {
            if (objects.Count > 0)
            {
                foreach (var obj in objects)
                {
                    if (obj == null)
                        FindObjectsWithRenderers();

                    if (false == _showInactive && false == obj.activeInHierarchy)
                        continue;

                    GUILayout.BeginHorizontal();

                    Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                    GUILayoutOption[] options = { GUILayout.Height(20) };

                    GUIContent content = new GUIContent(obj.name, icon);

                    GUIStyle labelStyle = new GUIStyle(GUI.skin.button);
                    if (PrefabUtility.IsPartOfAnyPrefab(obj))
                    {
                        if (obj.activeInHierarchy)
                        {
                            labelStyle.normal.textColor = Color.cyan;
                        }
                        else
                        {
                            labelStyle.normal.textColor = Color.gray;
                            labelStyle.fontStyle = FontStyle.Italic;
                        }
                    }
                    else if (!obj.activeInHierarchy)
                    {
                        labelStyle.normal.textColor = Color.gray;
                        labelStyle.fontStyle = FontStyle.Italic;
                    }

                    if (GUILayout.Button(content, labelStyle, options))
                    {
                        SelectAndFocusObject(obj);
                    }

                    using (new EditorGUI.DisabledScope(obj.GetComponent<PlanarShadow>() != null))
                    {
                        if (GUILayout.Button("그림자 추가", GUILayout.Width(100)))
                        {
                            PlanarShadow shadow = obj.AddComponent<PlanarShadow>();

                            shadow.Editor_FindComponents();
                            shadow.Editor_AddPlanarShadowMaterial();
                        }
                    }

                    using (new EditorGUI.DisabledScope(obj.GetComponent<PlanarShadow>() == null))
                    {
                        if (GUILayout.Button("그림자 제거", GUILayout.Width(100)))
                        {
                            PlanarShadow shadow = obj.GetComponent<PlanarShadow>();
                            shadow.Editor_RemovePlanarShadowMaterial();
                            if (shadow != null)
                            {
                                DestroyImmediate(shadow);
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("찾은 내용이 없습니다.");
            }
        }

        private static void SelectAndFocusObject(GameObject obj)
        {
            Selection.activeGameObject = obj;
            SceneView.lastActiveSceneView.FrameSelected();
        }

        private static Texture2D GetIcon(string iconName)
        {
            return EditorGUIUtility.IconContent(iconName).image as Texture2D;
        }
    }
}