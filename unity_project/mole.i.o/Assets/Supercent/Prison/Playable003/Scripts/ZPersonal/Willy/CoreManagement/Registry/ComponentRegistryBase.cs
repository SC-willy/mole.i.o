using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
#endif // UNITY_EDITOR

namespace Supercent.PrisonLife.Playable003
{

    public abstract class ComponentRegistryBase : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] bool _isBindManaged = false;
        public bool IsBindManaged => _isBindManaged;

        [ContextMenu("Regist")]
        public void BindSerializedField()
        {
            UnityEditor.Undo.RecordObject(this, "Regist");
            OnBindSerializedField();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        protected virtual void OnBindSerializedField() { }
        public virtual void OnInspectorGUI() { }
        public virtual void OnSceneGUI() { }

#endif // UNITY_EDITOR

    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ComponentRegistryBase), true)]
    [UnityEditor.CanEditMultipleObjects]
    [ExecuteInEditMode]
    public class ComponentRegistryEditor : UnityEditor.Editor
    {
        public static readonly MethodInfo REGIST_SERIALIZED_FIELD = typeof(ComponentRegistryBase).GetMethod("BindSerializedField", BindingFlags.Public | BindingFlags.Instance);
        public override void OnInspectorGUI()
        {
            var targetList = serializedObject.targetObjects;
            if (null == targetList)
                return;

            if (GUILayout.Button("Regist"))
            {
                for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
                {
                    var target = targetList[n] as ComponentRegistryBase;
                    if (null == target)
                        continue;

                    REGIST_SERIALIZED_FIELD.Invoke(target, null);
                }

                ComponentRegistryBase[] behaviorBases = FindObjectsOfType<ComponentRegistryBase>(true);
                for (int i = 0; i < behaviorBases.Length; i++)
                {
                    if (behaviorBases[i].IsBindManaged)
                    {
                        behaviorBases[i].BindSerializedField();
                    }
                }
            }

            CustomInspectorGUI();

            for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
            {
                var target = targetList[n] as ComponentRegistryBase;
                if (null == target)
                    continue;

                target.OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }
        protected virtual void CustomInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        void OnSceneGUI()
        {
            if (null == target)
                return;

            var bb = target as ComponentRegistryBase;
            bb.OnSceneGUI();
        }
    }
#endif // UNITY_EDITOR
}