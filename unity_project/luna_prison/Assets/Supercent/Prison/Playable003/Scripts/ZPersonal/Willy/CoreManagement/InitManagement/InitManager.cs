using System;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [Serializable]
    public class InitManager : IBindable, IStartable
    {
        [SerializeField] InitManagedObject[] _initializableComponents = null;

        public void StartSetup()
        {
            for (int i = 0; i < _initializableComponents.Length; i++)
            {
                _initializableComponents[i].Init();
            }
        }

        public static void InvokeInit(IInitable managed) => managed.Init();

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _initializableComponents = null;
            _initializableComponents = UnityEngine.Object.FindObjectsOfType<InitManagedObject>(true);
            Array.Sort(_initializableComponents);

            InitManagedBehaviorBase[] behaviorBases = UnityEngine.Object.FindObjectsOfType<InitManagedBehaviorBase>(true);
            for (int i = 0; i < behaviorBases.Length; i++)
            {
                if (behaviorBases[i].IsBindByManager)
                {
                    behaviorBases[i].BindSerializedField();
                }
            }
        }
#endif //UNITY_EDITOR
    }
}
