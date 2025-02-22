using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
#endif // UNITY_EDITOR

namespace Supercent.MoleIO.InGame
{
    public abstract class InitManagedBehaviorBase : InitManagedObject
    {

#if UNITY_EDITOR
        [SerializeField] bool _isBindByManager = false;
        public bool IsBindByManager => _isBindByManager;

        [ContextMenu("Bind")]
        public void BindSerializedField()
        {
            UnityEditor.Undo.RecordObject(this, "Bind");
            OnBindSerializedField();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        protected virtual void OnBindSerializedField() { }
        public virtual void OnInspectorGUI() { }
        public virtual void OnSceneGUI() { }
#endif // UNITY_EDITOR

    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(InitManagedBehaviorBase), true)]
    [UnityEditor.CanEditMultipleObjects]
    [ExecuteInEditMode]
    public class InitializableBehaviourBaseEditor : UnityEditor.Editor
    {
        public static readonly MethodInfo BIND_SERIALIZED_FIELD = typeof(InitManagedBehaviorBase).GetMethod("BindSerializedField", BindingFlags.Public | BindingFlags.Instance);

        public override void OnInspectorGUI()
        {
            var targetList = serializedObject.targetObjects;
            if (null == targetList)
                return;

            if (GUILayout.Button("Bind"))
            {
                for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
                {
                    var target = targetList[n] as InitManagedBehaviorBase;
                    if (null == target)
                        continue;

                    BIND_SERIALIZED_FIELD.Invoke(target, null);
                }
            }

            CustomInspectorGUI();

            for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
            {
                var target = targetList[n] as InitManagedBehaviorBase;
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

            var bb = target as InitManagedBehaviorBase;
            bb.OnSceneGUI();
        }
    }
#endif // UNITY_EDITOR
}
