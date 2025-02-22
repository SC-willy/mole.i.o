
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR


namespace Supercent.PrisonLife.Playable003
{
    public class CustomColorAttribute : PropertyAttribute
    {
        public Color Color;

        public CustomColorAttribute(float r, float g, float b)
        {
            Color = new Color(r, g, b);
        }
        public CustomColorAttribute(Color color)
        {
            Color = color;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CustomColorAttribute))]
    public class CustomColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var colorAttribute = (CustomColorAttribute)attribute;
            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.DrawRect(headerRect, colorAttribute.Color);

            headerRect.xMin += 3;
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            Rect outlineRect = new Rect(headerRect.x + 1, headerRect.y + 1, headerRect.width, headerRect.height);

            GUIStyle outlineStyle = new GUIStyle(style)
            {
                normal = { textColor = Color.black }
            };

            EditorGUI.LabelField(outlineRect, label, outlineStyle);
            EditorGUI.LabelField(headerRect, label, style);

            EditorGUI.PropertyField(position, property, GUIContent.none, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
#endif //UNITY_EDITOR

}
