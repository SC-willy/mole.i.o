using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif// UNITY_EDITOR

namespace Supercent.Base
{
    public abstract class ConfigBase : MonoBehaviour
    {
        protected virtual void Awake() { }
        protected virtual void OnDestroy() {}



#if UNITY_EDITOR
        [Serializable]
        class SectionInfo
        {
            public string name;
            public List<FieldInfo> fields = new List<FieldInfo>();
        }
        [Serializable]
        struct FieldInfo
        {
            public string name;
            public string type;
            public string tooltip;
        }

        [SerializeField] SectionInfo[] edit_schema;
        bool edit_isFirst = true;

        protected virtual void Reset()
        {
            name = GetType().Name;
        }
        protected virtual void OnValidate()
        {
            if (edit_isFirst)
            {
                edit_isFirst = false;
                edit_schema = default;
                EditorUtility.SetDirty(this);
            }
        }

        [MenuItem("CONTEXT/ConfigBase/Copy Infos", priority = 1001)]
        static void Edit_CopyInfos(MenuCommand menuCommand)
        {
            if (menuCommand.context is not ConfigBase comp) return;

            var message = string.Empty;
            if (Edit_FindFields(comp, out var result))
            {
                var sb = new StringBuilder();
                var map = Edit_MakeSections(result);
                foreach (var section in map.Values)
                {
                    var fields = section.fields;
                    if (fields.Count < 1) continue;

                    sb.Append($"### {section.name}\n\n");
                    for (int index = 0; index < fields.Count; ++index)
                    {
                        var field = fields[index];
                        sb.Append($"* {field.name} : {field.tooltip}\n\n");
                    }
                    sb.Append("\n\n");
                }
                message = sb.ToString();
            }
            EditorGUIUtility.systemCopyBuffer = message;
        }

        [MenuItem("CONTEXT/ConfigBase/Load Schema", priority = 1002)]
        static void Edit_LoadSchema(MenuCommand menuCommand)
        {
            if (Application.isPlaying) return;
            if (menuCommand.context is not ConfigBase comp) return;


            if (Edit_FindFields(comp, out var result))
            {
                var sections = Edit_MakeSections(result);
                comp.edit_schema = new SectionInfo[sections.Count];
                {
                    int index = 0;
                    foreach (var section in sections.Values)
                        comp.edit_schema[index++] = section;
                }
            }
            else
            {
                comp.edit_schema = new SectionInfo[0];
            }
            EditorUtility.SetDirty(comp);
        }

        [MenuItem("CONTEXT/ConfigBase/Apply Schema", priority = 1003)]
        static void Edit_ApplySchmea(MenuCommand menuCommand)
        {
            if (Application.isPlaying) return;
            if (menuCommand.context is not ConfigBase comp) return;

            var result = EditorUtility.DisplayDialog("Info", "Are you sure you want to apply the schema?", "yes", "no");
            if (!result)
                return;

            if (Edit_ScriptGeneration(comp, comp.edit_schema))
            {
                EditorUtility.DisplayDialog("Info", "Schema applied to the script successfully", "ok");
                comp.edit_schema = default;
                AssetDatabase.Refresh();
            }
            EditorUtility.SetDirty(comp);
        }


        static bool Edit_FindFields(Component src, out List<System.Reflection.FieldInfo> result)
        {
            if (src == null)
            {
                result = null;
                return false;
            }

            result = new List<System.Reflection.FieldInfo>();
            var fields = src.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var typeStr = typeof(string);
            for (int index = 0; index < fields.Length; ++index)
            {
                var item = fields[index];
                if (item == null) continue;
                if (item.IsSpecialName) continue;
                if (item.GetCustomAttribute<SerializeField>() == null) continue;
                if (item.GetCustomAttribute<LunaPlaygroundFieldAttribute>() == null) continue;

                result.Add(item);
            }
            return 0 < result.Count;
        }

        static Dictionary<string, SectionInfo> Edit_MakeSections(List<System.Reflection.FieldInfo> infos)
        {
            var result = new Dictionary<string, SectionInfo>();
            for (int index = 0; index < infos.Count; ++index)
            {
                var field = infos[index];
                if (field == null) continue;

                var luna = field.GetCustomAttribute<LunaPlaygroundFieldAttribute>();
                if (luna == null) continue;
                if (string.IsNullOrEmpty(luna.FieldSection)) continue;

                var useShortName = string.Compare(field.FieldType.Namespace, "System", true) == 0
                                || string.Compare(field.FieldType.Namespace, "UnityEngine", true) == 0;
                var typeName = useShortName ? field.FieldType.Name : field.FieldType.FullName;
                var info = new FieldInfo()
                {
                    type = typeName.Replace('+', '.'),
                    name = luna.FieldTitle,
                    tooltip = luna.FieldTooltip,
                };

                if (!result.TryGetValue(luna.FieldSection, out var section))
                    result[luna.FieldSection] = section = new SectionInfo() { name = luna.FieldSection };
                section.fields.Add(info);
            }
            return result;
        }

        static bool Edit_ScriptGeneration(MonoBehaviour obj, IList<SectionInfo> schema)
        {
            var mono = MonoScript.FromMonoBehaviour(obj);
            if (mono == null) return false;

            var path = AssetDatabase.GetAssetPath(mono);
            if (string.IsNullOrEmpty(path)) return false;

            var fullPath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), path);
            if (string.IsNullOrEmpty(fullPath)) return false;

            var index = fullPath.LastIndexOf(".cs");
            if (index < 0) return false;

            var type = obj.GetType();
            if (Edit_ConfigPartialGeneration(type, schema, out var script))
            {
                var extPartial = ".p";
                var savePath = fullPath.Insert(index, extPartial);

                Edit_SetPartial(type, fullPath);
                File.WriteAllText(savePath, script, Encoding.UTF8);
                return true;
            }
            return false;
        }

        static void Edit_SetPartial(Type type, string fullPath)
        {
            if (type == null) return;
            if (string.IsNullOrEmpty(fullPath)) return;

            var script = File.ReadAllText(fullPath, Encoding.UTF8);
            if (string.IsNullOrEmpty(script))
                return;

            var indexName = script.IndexOf(type.Name);
            if (indexName < 0)
                return;

            var indexPartial = script.LastIndexOf("partial", indexName, indexName + 1, StringComparison.Ordinal);
            if (indexPartial < 0)
            {
                var indexClass = script.LastIndexOf("class", indexName, indexName + 1, StringComparison.OrdinalIgnoreCase);
                if (-1 < indexClass)
                    File.WriteAllText(fullPath, script.Insert(indexClass, "partial "), Encoding.UTF8);
            }
        }

        static bool Edit_ConfigPartialGeneration(Type type, IList<SectionInfo> schema, out string result)
        {
            result = string.Empty;
            if (type == null)
                return false;

            bool hasNamespace = !string.IsNullOrEmpty(type.Namespace);
            var S4 = "    ";
            var NS4 = hasNamespace ? S4 : string.Empty;

            var sb = new StringBuilder();
            {
                sb.AppendLine("using System;");
                sb.AppendLine("using UnityEngine;");
                sb.AppendLine();
                if (hasNamespace)
                {
                    sb.AppendLine($"namespace {type.Namespace}");
                    sb.AppendLine("{");
                }
                {
                    sb.AppendLine($"{NS4}public partial class {type.Name}");
                    sb.AppendLine($"{NS4}{{");
                    {
                        for (int iSection = 0; iSection < schema.Count; ++iSection)
                        {
                            var section = schema[iSection];
                            if (section.fields.Count < 1)
                                continue;

                            if (0 < iSection)
                                sb.AppendLine().AppendLine();
                            var iOrder = 10 + iSection;
                            sb.AppendLine($"{NS4}{S4}[LunaPlaygroundSection(\"{section.name}\", {iOrder})][Header(\"{section.name}\")]");
                            for (int iField = 0; iField < section.fields.Count; ++iField)
                            {
                                if (0 < iField)
                                    sb.AppendLine();

                                var field = section.fields[iField];
                                sb.AppendLine($"{NS4}{S4}[LunaPlaygroundField(\"{field.name}\", {iField}, \"{section.name}\", false, \"{field.tooltip}\")][Tooltip(\"{field.tooltip}\")]");
                                sb.AppendLine($"{NS4}{S4}[SerializeField] {field.type} _{section.name}_{field.name};");
                                sb.AppendLine($"{NS4}{S4}public static {field.type} {section.name}_{field.name} => instance._{section.name}_{field.name};");
                            }
                        }
                    }
                    sb.AppendLine($"{NS4}}}");
                }
                if (hasNamespace)
                    sb.AppendLine("}");
            }
            result = sb.ToString();
            return true;
        }
#endif// UNITY_EDITOR
    }

    public abstract class Config<T> : ConfigBase where T : ConfigBase
    {
        protected static T instance { private set; get; } = null;

        protected override void Awake()
        {
#if UNITY_EDITOR
            if (instance != null)
                Debug.LogAssertion($"{nameof(Config<T>)} : An instance of type {typeof(T).Name} is already prepared");
#endif// UNITY_EDITOR
            instance = this as T;
        }
        protected override void OnDestroy()
        {
            if (instance == this as T)
                instance = null;
        }
    }
}