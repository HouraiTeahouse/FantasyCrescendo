using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    public abstract class PrefDrawer : PropertyDrawer {

        SerializedProperty GetKey(SerializedProperty property) {
            return property.FindPropertyRelative("_key");
        }

        SerializedProperty GetDefaultValue(SerializedProperty property) {
            return property.FindPropertyRelative("_defaultValue");
        }
             
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty key = GetKey(property);
            SerializedProperty defaultValue = GetDefaultValue(property);
            Rect foldoutRect = position;
            foldoutRect.width = 1;
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);
            position.x += foldoutRect.width;
            position.width -= foldoutRect.width;
            position.height = EditorGUI.GetPropertyHeight(key);
            EditorGUI.PropertyField(position, GetKey(property), new GUIContent(label.text + " Key"));
            if(property.isExpanded) {
                position.y += EditorGUI.GetPropertyHeight(defaultValue);
                EditorGUI.PropertyField(position, defaultValue);
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float baseHeight = EditorGUI.GetPropertyHeight(GetKey(property));
            if (property.isExpanded)
                baseHeight += EditorGUI.GetPropertyHeight(GetDefaultValue(property));
            return baseHeight;
        }
    }

    [CustomPropertyDrawer(typeof(PrefInt))]
    internal sealed class PrefIntDrawer : PrefDrawer {
    }
    
    [CustomPropertyDrawer(typeof(PrefFloat))]
    internal sealed class PrefFloatDrawer : PrefDrawer {
    }

    [CustomPropertyDrawer(typeof(PrefString))]
    internal sealed class PerfStringDrawer : PrefDrawer {
    }

    [CustomPropertyDrawer(typeof(PrefBool))]
    internal sealed class PrefBoolDrawer : PrefDrawer {
    }
}
