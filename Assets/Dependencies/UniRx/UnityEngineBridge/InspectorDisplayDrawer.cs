using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UniRx {

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InspectorDisplayAttribute : PropertyAttribute {

        public InspectorDisplayAttribute(string fieldName = "value", bool notifyPropertyChanged = true) {
            FieldName = fieldName;
            NotifyPropertyChanged = notifyPropertyChanged;
        }

        public string FieldName { get; private set; }
        public bool NotifyPropertyChanged { get; private set; }

    }

    /// <summary> Enables multiline input field for StringReactiveProperty. Default line is 3. </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MultilineReactivePropertyAttribute : PropertyAttribute {

        public MultilineReactivePropertyAttribute() { Lines = 3; }

        public MultilineReactivePropertyAttribute(int lines) { Lines = lines; }

        public int Lines { get; private set; }

    }

    /// <summary> Enables range input field for Int/FloatReactiveProperty. </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class RangeReactivePropertyAttribute : PropertyAttribute {

        public RangeReactivePropertyAttribute(float min, float max) {
            Min = min;
            Max = max;
        }

        public float Min { get; private set; }
        public float Max { get; private set; }

    }

#if UNITY_EDITOR

    // InspectorDisplay and for Specialized ReactiveProperty
    // If you want to customize other specialized ReactiveProperty
    // [UnityEditor.CustomPropertyDrawer(typeof(YourSpecializedReactiveProperty))]
    // public class ExtendInspectorDisplayDrawer : InspectorDisplayDrawer { } 

    [CustomPropertyDrawer(typeof(InspectorDisplayAttribute))]
    [CustomPropertyDrawer(typeof(IntReactiveProperty))]
    [CustomPropertyDrawer(typeof(LongReactiveProperty))]
    [CustomPropertyDrawer(typeof(ByteReactiveProperty))]
    [CustomPropertyDrawer(typeof(FloatReactiveProperty))]
    [CustomPropertyDrawer(typeof(DoubleReactiveProperty))]
    [CustomPropertyDrawer(typeof(StringReactiveProperty))]
    [CustomPropertyDrawer(typeof(BoolReactiveProperty))]
    [CustomPropertyDrawer(typeof(Vector2ReactiveProperty))]
    [CustomPropertyDrawer(typeof(Vector3ReactiveProperty))]
    [CustomPropertyDrawer(typeof(Vector4ReactiveProperty))]
    [CustomPropertyDrawer(typeof(ColorReactiveProperty))]
    [CustomPropertyDrawer(typeof(RectReactiveProperty))]
    [CustomPropertyDrawer(typeof(AnimationCurveReactiveProperty))]
    [CustomPropertyDrawer(typeof(BoundsReactiveProperty))]
    [CustomPropertyDrawer(typeof(QuaternionReactiveProperty))]
    public class InspectorDisplayDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            string fieldName;
            bool notifyPropertyChanged;
            {
                var attr = attribute as InspectorDisplayAttribute;
                fieldName = attr == null ? "value" : attr.FieldName;
                notifyPropertyChanged = attr == null ? true : attr.NotifyPropertyChanged;
            }

            if (notifyPropertyChanged) {
                EditorGUI.BeginChangeCheck();
            }
            SerializedProperty targetSerializedProperty = property.FindPropertyRelative(fieldName);
            if (targetSerializedProperty == null) {
                EditorGUI.LabelField(position,
                    label,
                    new GUIContent() {text = "InspectorDisplay can't find target:" + fieldName});
                if (notifyPropertyChanged) {
                    EditorGUI.EndChangeCheck();
                }
                return;
            }
            else {
                EmitPropertyField(position, targetSerializedProperty, label);
            }

            if (notifyPropertyChanged) {
                if (EditorGUI.EndChangeCheck()) {
                    property.serializedObject.ApplyModifiedProperties(); // deserialize to field

                    string[] paths = property.propertyPath.Split('.'); // X.Y.Z...
                    Object attachedComponent = property.serializedObject.targetObject;

                    object targetProp = paths.Length == 1
                        ? fieldInfo.GetValue(attachedComponent)
                        : GetValueRecursive(attachedComponent, 0, paths);
                    if (targetProp == null)
                        return;
                    PropertyInfo propInfo = targetProp.GetType()
                        .GetProperty(fieldName,
                            BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Instance
                                | BindingFlags.Public | BindingFlags.NonPublic);
                    object modifiedValue = propInfo.GetValue(targetProp, null); // retrieve new value

                    MethodInfo methodInfo = targetProp.GetType()
                        .GetMethod("SetValueAndForceNotify",
                            BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Instance
                                | BindingFlags.Public | BindingFlags.NonPublic);
                    if (methodInfo != null) {
                        methodInfo.Invoke(targetProp, new object[] {modifiedValue});
                    }
                }
                else {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        object GetValueRecursive(object obj, int index, string[] paths) {
            string path = paths[index];
            FieldInfo fieldInfo = obj.GetType()
                .GetField(path,
                    BindingFlags.IgnoreCase | BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public
                        | BindingFlags.NonPublic);

            // If array, path = Array.data[index]
            if (fieldInfo == null && path == "Array") {
                try {
                    path = paths[++index];
                    Match m = Regex.Match(path, @"(.+)\[([0-9]+)*\]");
                    int arrayIndex = int.Parse(m.Groups[2].Value);
                    object arrayValue = (obj as IList)[arrayIndex];
                    if (index < paths.Length - 1) {
                        return GetValueRecursive(arrayValue, ++index, paths);
                    }
                    else {
                        return arrayValue;
                    }
                } catch {
                    Debug.Log("InspectorDisplayDrawer Exception, objType:" + obj.GetType().Name + " path:"
                        + string.Join(", ", paths));
                    throw;
                }
            }
            else if (fieldInfo == null) {
                throw new Exception("Can't decode path, please report to UniRx's GitHub issues:"
                    + string.Join(", ", paths));
            }

            object v = fieldInfo.GetValue(obj);
            if (index < paths.Length - 1) {
                return GetValueRecursive(v, ++index, paths);
            }

            return v;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var attr = attribute as InspectorDisplayAttribute;
            string fieldName = attr == null ? "value" : attr.FieldName;

            float height = base.GetPropertyHeight(property, label);
            SerializedProperty valueProperty = property.FindPropertyRelative(fieldName);
            if (valueProperty == null) {
                return height;
            }

            if (valueProperty.propertyType == SerializedPropertyType.Rect) {
                return height * 2;
            }
            if (valueProperty.propertyType == SerializedPropertyType.Bounds) {
                return height * 3;
            }
            if (valueProperty.propertyType == SerializedPropertyType.String) {
                MultilineReactivePropertyAttribute multilineAttr = GetMultilineAttribute();
                if (multilineAttr != null) {
                    return (!EditorGUIUtility.wideMode ? 16f : 0f) + 16f + (float) ((multilineAttr.Lines - 1) * 13);
                }
                ;
            }

            if (valueProperty.isExpanded) {
                var count = 0;
                IEnumerator e = valueProperty.GetEnumerator();
                while (e.MoveNext())
                    count++;
                return (height + 4) * count + 6; // (Line = 20 + Padding) ?
            }

            return height;
        }

        protected virtual void EmitPropertyField(Rect position,
                                                 SerializedProperty targetSerializedProperty,
                                                 GUIContent label) {
            MultilineReactivePropertyAttribute multiline = GetMultilineAttribute();
            if (multiline == null) {
                RangeReactivePropertyAttribute range = GetRangeAttribute();
                if (range == null) {
                    EditorGUI.PropertyField(position, targetSerializedProperty, label, true);
                }
                else {
                    if (targetSerializedProperty.propertyType == SerializedPropertyType.Float) {
                        EditorGUI.Slider(position, targetSerializedProperty, range.Min, range.Max, label);
                    }
                    else if (targetSerializedProperty.propertyType == SerializedPropertyType.Integer) {
                        EditorGUI.IntSlider(position, targetSerializedProperty, (int) range.Min, (int) range.Max, label);
                    }
                    else {
                        EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
                    }
                }
            }
            else {
                SerializedProperty property = targetSerializedProperty;

                label = EditorGUI.BeginProperty(position, label, property);
                MethodInfo method = typeof(EditorGUI).GetMethod("MultiFieldPrefixLabel",
                    BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic);
                position = (Rect) method.Invoke(null, new object[] {position, 0, label, 1});

                EditorGUI.BeginChangeCheck();
                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                string stringValue = EditorGUI.TextArea(position, property.stringValue);
                EditorGUI.indentLevel = indentLevel;
                if (EditorGUI.EndChangeCheck()) {
                    property.stringValue = stringValue;
                }
                EditorGUI.EndProperty();
            }
        }

        MultilineReactivePropertyAttribute GetMultilineAttribute() {
            FieldInfo fi = fieldInfo;
            if (fi == null)
                return null;
            return fi.GetCustomAttributes(false).OfType<MultilineReactivePropertyAttribute>().FirstOrDefault();
        }

        RangeReactivePropertyAttribute GetRangeAttribute() {
            FieldInfo fi = fieldInfo;
            if (fi == null)
                return null;
            return fi.GetCustomAttributes(false).OfType<RangeReactivePropertyAttribute>().FirstOrDefault();
        }

    }

#endif
}