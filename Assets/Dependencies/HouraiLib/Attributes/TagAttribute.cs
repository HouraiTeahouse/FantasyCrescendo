using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HouraiTeahouse {

    /// <summary> A propperty attribute that provides a Tag selector for strings. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TagAttribute : PropertyAttribute {

    }

#if UNITY_EDITOR
    /// <summary> Custom PropertyDrawer for TagAttribute </summary>
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagAttributeDrawer : PropertyDrawer {

        /// <summary>
        ///     <see cref="PropertyDrawer.OnGUI" />
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            EditorGUI.BeginProperty(position, label, property);
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            EditorGUI.EndProperty();
        }

    }
#endif
}