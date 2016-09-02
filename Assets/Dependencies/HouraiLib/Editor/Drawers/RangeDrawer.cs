using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    [CustomPropertyDrawer(typeof(Range))]
    public class RangeDrawer : PropertyDrawer {

        readonly GUIContent[] _content = {new GUIContent("-"), new GUIContent("+")};

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            SerializedProperty min = property.FindPropertyRelative("_min");
            SerializedProperty max = property.FindPropertyRelative("_max");
            var values = new[] {min.floatValue, max.floatValue};
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(position, label, _content, values);
            if (!EditorGUI.EndChangeCheck())
                return;
            max.floatValue = Mathf.Max(values);
            min.floatValue = Mathf.Min(values);
        }

    }

}