using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Custom PropertyDrawer for ReadOnlyAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer {

        /// <summary>
        /// <see cref="PropertyDrawer.OnGUI"/>
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            string value;
            switch (property.propertyType) {
                case SerializedPropertyType.String:
                    value = property.stringValue;
                    break;
                case SerializedPropertyType.Float:
                    value = property.floatValue.ToString();
                    break;
                case SerializedPropertyType.Integer:
                    value = property.intValue.ToString();
                    break;
                default:
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property);
                    GUI.enabled = true;
                    return;
            }
            EditorGUI.LabelField(position, label, new GUIContent(value));
        }

    }
}
