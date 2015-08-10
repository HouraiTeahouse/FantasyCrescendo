using UnityEditor;
using UnityEngine;

namespace Crescendo.API.Editor {

    [CustomPropertyDrawer(typeof (MinMaxSliderAttribute))]
    internal class MinMaxSliderDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType == SerializedPropertyType.Vector2) {
                Vector2 range = property.vector2Value;
                float min = range.x;
                float max = range.y;
                var attr = attribute as MinMaxSliderAttribute;
                Rect slider, minR, maxR;
                slider = position;
                minR = position;
                maxR = position;
                minR.width = 0.1f*slider.width;
                maxR.width = minR.width;
                slider.width -= minR.width + maxR.width;
                minR.x = slider.x + slider.width;
                maxR.x = minR.x + minR.width;
                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(label, slider, ref min, ref max, attr.min, attr.max);
                min = EditorGUI.FloatField(minR, GUIContent.none, min);
                max = EditorGUI.FloatField(maxR, GUIContent.none, max);
                if (!EditorGUI.EndChangeCheck())
                    return;
                range.x = min;
                range.y = max;
                property.vector2Value = range;
            } else
                EditorGUI.LabelField(position, label, "Use only with Vector2");
        }

    }

}