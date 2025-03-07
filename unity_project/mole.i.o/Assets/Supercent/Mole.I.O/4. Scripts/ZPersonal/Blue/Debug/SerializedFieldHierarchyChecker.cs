#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public static class SerializedFieldHierarchyChecker
{
    [MenuItem("Tools/Blue/대 륙 횡 단 바 인 딩 검 사")]
    public static void CheckSerializedFieldsInScene()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(true);
        List<string> issues = new List<string>();

        foreach (GameObject obj in allObjects)
        {
            MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component == null) continue; // Missing Script 무시

                FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    if (field.GetCustomAttribute<SerializeField>() != null)
                    {
                        Object fieldValue = field.GetValue(component) as Object;

                        // 필드가 null이거나 프로젝트 에셋인 경우 스킵
                        if (fieldValue == null || AssetDatabase.Contains(fieldValue))
                        {
                            continue;
                        }

                        if (fieldValue is GameObject fieldObject)
                        {
                            if (!IsChildOrSelf(obj.transform, fieldObject.transform))
                            {
                                issues.Add($"오브젝트 '{obj.name}'의 '{field.Name}' 필드가 자신의 부모-자식 관계 외부에 있는 '{fieldObject.name}'을(를) 참조하고 있습니다.");
                            }
                        }
                        else if (fieldValue is Component fieldComponent)
                        {
                            if (!IsChildOrSelf(obj.transform, fieldComponent.transform))
                            {
                                issues.Add($"오브젝트 '{obj.name}'의 '{field.Name}' 필드가 자신의 부모-자식 관계 외부에 있는 '{fieldComponent.name}'을(를) 참조하고 있습니다.");
                            }
                        }
                    }
                }
            }
        }

        if (issues.Count > 0)
        {
            Debug.Log("=== SerializeField 바인딩 검사 결과 ===");
            foreach (string issue in issues)
            {
                Debug.LogWarning(issue);
            }
        }
        else
        {
            Debug.Log("문제 없음: 모든 SerializeField 필드가 올바르게 바인딩되어 있습니다.");
        }
    }

    private static bool IsChildOrSelf(Transform parent, Transform target)
    {
        Transform current = target;
        while (current != null)
        {
            if (current == parent) return true;
            current = current.parent;
        }
        return false;
    }
}
#endif