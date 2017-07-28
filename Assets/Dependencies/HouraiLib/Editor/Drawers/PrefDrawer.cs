using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    public abstract class PrefDrawer : PropertyDrawer {

        SerializedProperty GetKey(SerializedProperty property) { return property.FindPropertyRelative("_key"); }

        SerializedProperty GetDefaultValue(SerializedProperty property) {
            return property.FindPropertyRelative("_defaultValue");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            SerializedProperty key = GetKey(property);
            SerializedProperty defaultValue = GetDefaultValue(property);
            using (hGUI.Property(label, position, GetKey(property))) {
                position.width /= 2;
                EditorGUI.PropertyField(position, key, label);
                position.x += position.width;
                EditorGUI.PropertyField(position, defaultValue, GUIContent.none);
            }
        }

    }

    [CustomPropertyDrawer(typeof(PrefInt))]
    internal sealed class PrefIntDrawer : PrefDrawer {

    }

    [CustomPropertyDrawer(typeof(PrefFloat))]
    internal sealed class PrefFloatDrawer : PrefDrawer {

    }

    [CustomPropertyDrawer(typeof(PrefString))]
    internal sealed class PrefStringDrawer : PrefDrawer {

    }

    [CustomPropertyDrawer(typeof(PrefBool))]
    internal sealed class PrefBoolDrawer : PrefDrawer {

    }

}