using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class LobbyManager : MonoBehaviour
    {
        [Header("Managers")]
        [CustomColor(0f, 0f, 0.2f)]
        [SerializeField] InitManager _initManager = new InitManager();
        [CustomColor(0.23f, 0.34f, 0f)]
        [SerializeField] TransitionManager _transitionManager = new TransitionManager();
        [CustomColor(0.2f, 0f, 0.1f)]
        [SerializeField] CameraManager _cameraManager = new CameraManager();

        private void Start()
        {
            InvokeStartMethod(_initManager);
            InvokeStartMethod(_transitionManager);
            InvokeStartMethod(_cameraManager);
        }

        private void Update()
        {
            _transitionManager.UpdateManualy(Time.deltaTime);
        }

        private void InvokeStartMethod(IStartable managed) => managed.StartSetup();

        public void CheckStartUp(Action<bool> subscriber) => subscriber?.Invoke(_initManager != null);

#if UNITY_EDITOR

        [ContextMenu("Binding Managers")]
        public void BindSerializedField()
        {
            UnityEditor.Undo.RecordObject(this, "Binding Managers");
            OnBindSerializedField();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        protected virtual void OnBindSerializedField()
        {
            Bind(_initManager);
            Bind(_transitionManager);
            Bind(_cameraManager);
        }
        protected void Bind(IBindable bindable) => bindable.Bind(this);
        public virtual void OnInspectorGUI() { }
        public virtual void OnSceneGUI() { }


#endif // UNITY_EDITOR
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(LobbyManager), true)]
    [UnityEditor.CanEditMultipleObjects]
    [ExecuteInEditMode]
    public class LobbyManagerManagerEditor : UnityEditor.Editor
    {
        public static readonly MethodInfo BIND_SERIALIZED_FIELD = typeof(LobbyManager).GetMethod("BindSerializedField", BindingFlags.Public | BindingFlags.Instance);

        public override void OnInspectorGUI()
        {
            var targetList = serializedObject.targetObjects;
            if (null == targetList)
                return;

            if (GUILayout.Button("Binding Managers"))
            {
                for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
                {
                    var target = targetList[n] as LobbyManager;
                    if (null == target)
                        continue;

                    BIND_SERIALIZED_FIELD.Invoke(target, null);
                }
            }

            CustomInspectorGUI();

            for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
            {
                var target = targetList[n] as LobbyManager;
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

            var bb = target as LobbyManager;
            bb.OnSceneGUI();
        }
    }
#endif // UNITY_EDITOR

}