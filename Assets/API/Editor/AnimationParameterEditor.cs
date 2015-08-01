using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Crescendo.API.Editor {

    [CustomPropertyDrawer(typeof(AnimationBool))]
    [CustomPropertyDrawer(typeof(AnimationInt))]
    [CustomPropertyDrawer(typeof(AnimationFloat))]
    [CustomPropertyDrawer(typeof(AnimationTrigger))]
    public class AnimationParameterDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty name = property.FindPropertyRelative("_name");
            SerializedProperty hash = property.FindPropertyRelative("_hash");

            EditorGUI.PropertyField(position, name, label);
            hash.intValue = Animator.StringToHash(name.stringValue);
        }

    }

}

