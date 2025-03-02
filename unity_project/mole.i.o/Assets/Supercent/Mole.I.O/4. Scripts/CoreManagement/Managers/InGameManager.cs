using System;
using System.Collections;
using System.Reflection;
using Supercent.Util;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class InGameManager : MonoBehaviour
    {
        public static LevelData CurLevelData;

        [Header("Managers")]
        [CustomColor(0f, 0f, 0.2f)]
        [SerializeField] InitManager _initManager = new InitManager();
        [CustomColor(0.23f, 0.34f, 0f)]
        [SerializeField] TransitionManager _transitionManager = new TransitionManager();
        [SerializeField] LevelData _levelData;

        [Header("Mediators")]
        [CustomColor(0.2f, 0.1f, 0.3f)]
        [SerializeField] IngameMainMediator _mainMed = new IngameMainMediator();

        private void Start()
        {
            if (CurLevelData == null)
                CurLevelData = _levelData;

            InvokeStartMethod(_initManager);
            InvokeStartMethod(_mainMed);
            InvokeStartMethod(_transitionManager);

            StartCoroutine(CoWaitForMove());
        }

        private IEnumerator CoWaitForMove()
        {
            yield return CoroutineUtil.WaitForSeconds(0.5f);
            _mainMed.StartGame();
        }

        private void Update()
        {
            _transitionManager.UpdateManualy(Time.deltaTime);
            _mainMed.UpdateManualy(Time.deltaTime);
        }

        private void InvokeStartMethod(IStartable managed) => managed.StartSetup();

        public void CheckStartUp(Action<bool> subscriber) => subscriber?.Invoke(_initManager != null);
        public void StartCamShow(int index) => _mainMed.ShowCamPos(index);

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
            Bind(_mainMed);
            Bind(_transitionManager);
        }
        protected void Bind(IBindable bindable) => bindable.Bind(this);
        public virtual void OnInspectorGUI() { }
        public virtual void OnSceneGUI() { }


#endif // UNITY_EDITOR
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(InGameManager), true)]
    [UnityEditor.CanEditMultipleObjects]
    [ExecuteInEditMode]
    public class InitManagerEditor : UnityEditor.Editor
    {
        public static readonly MethodInfo BIND_SERIALIZED_FIELD = typeof(InGameManager).GetMethod("BindSerializedField", BindingFlags.Public | BindingFlags.Instance);

        public override void OnInspectorGUI()
        {
            var targetList = serializedObject.targetObjects;
            if (null == targetList)
                return;

            if (GUILayout.Button("Binding Managers"))
            {
                for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
                {
                    var target = targetList[n] as InGameManager;
                    if (null == target)
                        continue;

                    BIND_SERIALIZED_FIELD.Invoke(target, null);
                }
            }

            CustomInspectorGUI();

            for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
            {
                var target = targetList[n] as InGameManager;
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

            var bb = target as InGameManager;
            bb.OnSceneGUI();
        }
    }
#endif // UNITY_EDITOR

}