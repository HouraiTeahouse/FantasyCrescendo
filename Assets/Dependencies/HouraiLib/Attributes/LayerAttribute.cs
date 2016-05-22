using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse {
    /// <summary>
    /// A PropertyAttribute that exposes a Layer control on the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class LayerAttribute : PropertyAttribute {
    }

#if UNITY_EDITOR
    /// <summary>
    /// Custom PropertyDrawer for LayerAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.Integer) {
                base.OnGUI(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
            EditorGUI.EndProperty();
        }
    }
#endif
}
