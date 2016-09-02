using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.Editor {

    /// <summary> Custom PropertyDrawer for ResourcePathAttribute. </summary>
    //TODO(jame7132): Move this to the Resource Attribute file
    [CustomPropertyDrawer(typeof(ResourceAttribute))]
    internal class ResourceAttributeDrawer : BasePropertyDrawer<ResourceAttribute> {

        class Data {

            Object _object;
            string _path;
            public readonly GUIContent Content;

            public Data(SerializedProperty property, GUIContent content) {
                _path = property.stringValue;
                _object = Resources.Load(_path);
                Content = new GUIContent(content);
                UpdateContent(content);
            }

            bool Valid {
                get { return !_path.IsNullOrEmpty(); }
            }

            public void Draw(Rect position, SerializedProperty property, Type type) {
                EditorGUI.BeginChangeCheck();
                Color oldColor = GUI.color;
                GUI.color = Valid ? GUI.color : Color.red;
                Object obj = EditorGUI.ObjectField(position, Content, _object, type, false);
                GUI.color = oldColor;
                if (!EditorGUI.EndChangeCheck())
                    return;
                Update(obj);
                property.stringValue = _path;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            public void UpdateContent(GUIContent label) {
                string message;
                if (!_object)
                    message = "No object specified";
                else if (!Valid)
                    message = "Not in a Resources folder. Will not be saved.";
                else
                    message = "Path: {0}".With(_path);

                Content.tooltip = label.tooltip.IsNullOrEmpty() ? message : "{0}\n{1}".With(label.tooltip, message);
            }

            void Update(Object obj) {
                _object = obj;
                _path = Assets.GetResourcePath(_object);
            }

        }

        readonly Dictionary<string, Data> _data;

        public ResourceAttributeDrawer() { _data = new Dictionary<string, Data>(); }

        /// <summary>
        ///     <see cref="PropertyDrawer.OnGUI" />
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            string propertyPath = property.propertyPath;
            bool changed = !_data.ContainsKey(propertyPath);
            Data data = changed ? new Data(property, label) : _data[propertyPath];

            using (hGUI.Property(data.Content, position, property)) {
                data.UpdateContent(label);
                data.Draw(position, property, attribute.TypeRestriction);
                _data[propertyPath] = data;
            }
        }

    }

}