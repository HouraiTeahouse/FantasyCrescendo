using System;
using System.Collections.Generic;
using System.Linq;
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
                _object = Assets.IsBundlePath(_path) ? Assets.LoadBundledAsset(_path) : Resources.Load(_path);
                Content = new GUIContent(content);
                UpdateContent(content);
            }

            bool Valid {
                get { return !_path.IsNullOrEmpty(); }
            }

            public void Draw(Rect position, SerializedProperty property, Type type) {
                EditorGUI.BeginChangeCheck();
                Object obj;
                using (hGUI.Color(Valid ? GUI.color : Color.red))
                    obj = EditorGUI.ObjectField(position, Content, _object, type, false);
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
                    message = "Not in Resources folder or Asset Bundle. Will not be saved.";
                else if (_path.IndexOf(Resource.BundleSeperator) >= 0) {
                    string[] splits = _path.Split(Resource.BundleSeperator);
                    message = "Asset Bundle: {0}\nPath:{1}".With(splits[0], splits[1]);
                } else 
                    message = "Path: {0}".With(_path);

                Content.tooltip = label.tooltip.IsNullOrEmpty() ? message : "{0}\n{1}".With(label.tooltip, message);
            }

            void Update(Object obj) {
                _object = obj;
                var bundleName = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_object)).assetBundleName;
                if (Assets.IsResource(_object))
                    _path = Assets.GetResourcePath(_object);
                else if (!string.IsNullOrEmpty(bundleName))
                    _path = "{0}:{1}".With(bundleName, _object.name);
                else
                    _path = string.Empty;
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
