using System;
using System.Collections;
using System.Reflection;
using Luna.Unity;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [CustomColor(0f, 0f, 0.2f)]
        [SerializeField] InitManager _initManager = new InitManager();

        [CustomColor(0.3f, 0.1f, 0.07f)]
        [SerializeField] AudioMuteManager _audioMuteManager = new AudioMuteManager();
        [CustomColor(0.7f, 0.4f, 0f)]
        [SerializeField] MoneyManager _moneyManager = new MoneyManager();
        [CustomColor(0.23f, 0.34f, 0f)]
        [SerializeField] TransitionManager _transitionManager = new TransitionManager();


        [Header("Mediators")]
        [CustomColor(0.2f, 0.1f, 0.3f)]
        [SerializeField] MainMediator _mainMed = new MainMediator();
        [SerializeField] GameObject[] _roomObjects;


        private void Start()
        {
            InvokeStartMethod(_initManager);
            InvokeStartMethod(_mainMed);
            InvokeStartMethod(_transitionManager);
            InitManager.InvokeInit(_moneyManager);
        }

        private void Update()
        {
            _transitionManager.UpdateManualy(Time.deltaTime);
        }

        private void InvokeStartMethod(IStartable managed) => managed.StartSetup();
        public void ActiveSound(bool on) => _audioMuteManager.ActiveSound(on);

        public void CheckStartUp(Action<bool> subscriber)
        {
            subscriber?.Invoke(_initManager != null);
        }

        public void StartCamShow(int index)
        {
            _mainMed.ShowCamPos(index);
        }

        public void SetOnUseMoney(Action action, bool _isRegist) => _moneyManager.SetOnUseMoney(action, _isRegist);

        public void CheckUnlock()
        {
            StartCoroutine(CoCheckUnlock());

        }

        private IEnumerator CoCheckUnlock()
        {
            yield return null;
            for (int i = 0; i < _roomObjects.Length; i++)
            {
                if (!_roomObjects[i].activeSelf)
                {
                    _mainMed.StartFlowWaitUpgrade();
                    yield break;
                }
            }
            _mainMed.StartMineFlow();
        }

        public void CheckUnlockPrevious()
        {
            for (int i = 0; i < _roomObjects.Length; i++)
            {
                if (!_roomObjects[i].activeSelf)
                {
                    return;
                }
            }
            _mainMed.StartMineFlow();
            _mainMed.StopAllFlow();
            _mainMed.StartMineFlow();
        }

        public void ShowMineArea() => _mainMed.OpenMineArea();

        public void EndGame()
        {
            LifeCycle.GameEnded();
            _mainMed.ShowFinalCam();
        }
        public void OpenCTA()
        {
            Playable.InstallFullGame();
        }

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
            Bind(_audioMuteManager);
            Bind(_moneyManager);
            Bind(_transitionManager);
        }
        protected void Bind(IBindable bindable) => bindable.Bind(this);
        public virtual void OnInspectorGUI() { }
        public virtual void OnSceneGUI() { }
#endif // UNITY_EDITOR
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GameManager), true)]
    [UnityEditor.CanEditMultipleObjects]
    [ExecuteInEditMode]
    public class InitManagerEditor : UnityEditor.Editor
    {
        public static readonly MethodInfo BIND_SERIALIZED_FIELD = typeof(GameManager).GetMethod("BindSerializedField", BindingFlags.Public | BindingFlags.Instance);

        public override void OnInspectorGUI()
        {
            var targetList = serializedObject.targetObjects;
            if (null == targetList)
                return;

            if (GUILayout.Button("Binding Managers"))
            {
                for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
                {
                    var target = targetList[n] as GameManager;
                    if (null == target)
                        continue;

                    BIND_SERIALIZED_FIELD.Invoke(target, null);
                }
            }

            CustomInspectorGUI();

            for (int n = 0, cnt = targetList.Length; n < cnt; ++n)
            {
                var target = targetList[n] as GameManager;
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

            var bb = target as GameManager;
            bb.OnSceneGUI();
        }
    }
#endif // UNITY_EDITOR

}