using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    public class AbstractRangeDrawer : PropertyDrawer {

        readonly GUIContent[] _content = {new GUIContent("-"), new GUIContent("+")};

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.MultiPropertyField(position, _content, property.FindPropertyRelative("_min"), label);
        }

    }

    [CustomPropertyDrawer(typeof(Range))]
    internal class RangeDrawer : AbstractRangeDrawer {
    }

    [CustomPropertyDrawer(typeof(IntRange))]
    internal class IntRangeDrawer : AbstractRangeDrawer {
    }

}
