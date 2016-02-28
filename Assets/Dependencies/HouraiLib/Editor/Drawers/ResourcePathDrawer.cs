using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Custom PropertyDrawer for ResourcePathAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof (ResourceAttribute))]
    internal class ResourceAttributeDrawer : PropertyDrawer {

        private class Data {
            private Object _object;
            private string _path;
            public readonly GUIContent Content;

            public Data(SerializedProperty property, GUIContent content) {
                _path = property.stringValue;
                _object = Resources.Load(_path);
                Content = new GUIContent(content);
                UpdateContent(content);
            }

            public void Draw(Rect position, SerializedProperty property, Type type) {
                EditorGUI.BeginChangeCheck();
                Object obj = EditorGUI.ObjectField(position, Content, _object, type, false);
                if (EditorGUI.EndChangeCheck()) {
                    Update(obj);
                    property.stringValue = _path;
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }

            public void UpdateContent(GUIContent label) {
                bool validPath = !string.IsNullOrEmpty(_path);
                Content.text = string.Format("{0} ({1})", label.text, validPath ? "\u2713" : "\u2715");
                string message;
                if (!_object)
                    message = "No object specified";
                else if (!validPath)
                    message = "Not in a Resources folder. Will not be saved.";
                else
                    message = string.Format("Path: {0}", _path);

                if (string.IsNullOrEmpty(label.tooltip))
                    Content.tooltip = message;
                else
                    Content.tooltip = string.Format("{0}\n{1}", label.tooltip, message);
            }

           void Update(Object obj) {
                _object = obj;
                _path = AssetUtil.GetResourcePath(_object);
            }
        }

        private readonly Dictionary<string, Data> _data;
        private ResourceAttribute _resourceAttribute;

        public ResourceAttributeDrawer() {
           _data = new Dictionary<string, Data>(); 
        }

        /// <summary>
        /// <see cref="PropertyDrawer.OnGUI"/>
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            if (_resourceAttribute == null)
                _resourceAttribute = attribute as ResourceAttribute;

            string propertyPath = property.propertyPath;
            Data data;
            bool changed = !_data.ContainsKey(propertyPath);
            if (changed) 
                data = new Data(property, label);
            else 
                data = _data[propertyPath];

            EditorGUI.BeginProperty(position, data.Content, property);
            data.UpdateContent(label);
            data.Draw(position, property, _resourceAttribute.TypeRestriction);
            _data[propertyPath] = data;
            EditorGUI.EndProperty();
        }

    }

}
