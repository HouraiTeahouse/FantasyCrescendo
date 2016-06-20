using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Custom PropertyDrawer for ResourcePathAttribute.
    /// </summary>
    //TODO(jame7132): Move this to the Resource Attribute file
    [CustomPropertyDrawer(typeof (ResourceAttribute))]
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

            public void Draw(Rect position, SerializedProperty property, Type type) {
                EditorGUI.BeginChangeCheck();
                Object obj = EditorGUI.ObjectField(position, Content, _object, type, false);
                if (!EditorGUI.EndChangeCheck()) return;
                Update(obj);
                property.stringValue = _path;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
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

        readonly Dictionary<string, Data> _data;

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

            string propertyPath = property.propertyPath;
            bool changed = !_data.ContainsKey(propertyPath);
            Data data = changed ? new Data(property, label) : _data[propertyPath];

            using (EditorUtil.Property(data.Content, position, property)) {
                data.UpdateContent(label);
                data.Draw(position, property, attribute.TypeRestriction);
                _data[propertyPath] = data;
            }
        }
    }
}
