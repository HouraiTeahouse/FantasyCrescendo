using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.Editor {

    /// <summary> Custom PropertyDrawer for SceneAttribute </summary>
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    internal class SceneAttributeDrawer : PropertyDrawer {

        readonly Dictionary<string, Data> _data;

        public SceneAttributeDrawer() { _data = new Dictionary<string, Data>(); }

        class Data {

            SceneAsset _object;
            string _path;
            public readonly GUIContent Content;

            public Data(SerializedProperty property, GUIContent content) {
                _path = property.stringValue;
                string path = "Assets/{0}.unity".With(_path);
                if (Assets.IsBundlePath(_path)) {
                    string[] parts = _path.Split(Resource.BundleSeperator);
                    var paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(parts[0], parts[1]);
                    path = paths.FirstOrDefault() ?? path;
                }
                _object = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                Content = new GUIContent(content);
                UpdateContent(content);
            }

            bool Valid {
                get { return !_path.IsNullOrEmpty(); }
            }

            public void Draw(Rect position, SerializedProperty property, Type type) {
                EditorGUI.BeginChangeCheck();
                SceneAsset obj;
                using (hGUI.Color(Valid ? GUI.color : Color.red))
                    obj = EditorGUI.ObjectField(position, Content, _object, type, false) as SceneAsset;
                if (!EditorGUI.EndChangeCheck())
                    return;
                Update(obj);
                property.stringValue = _path;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            public void UpdateContent(GUIContent label) {
                string message;
                if (!_object) {
                    message = "No object specified";
                } else if (!Valid) {
                    message = "Not in Build Settings or Asset Bundle. Will not be saved.";
                } else if (_path.IndexOf(Resource.BundleSeperator) >= 0) {
                    string[] splits = _path.Split(Resource.BundleSeperator);
                    message = "Asset Bundle: {0}\nPath:{1}".With(splits[0], splits[1]);
                } else {
                    message = "Path: {0}".With(_path);
                }

                Content.tooltip = label.tooltip.IsNullOrEmpty() ? message : "{0}\n{1}".With(label.tooltip, message);
            }

            void Update(SceneAsset obj) {
                _object = obj;
                var scenePath = Assets.GetScenePath(obj);
                var bundleName = Assets.GetBundlePath(obj);
                _path = scenePath ?? bundleName ?? string.Empty;
            }

        }


        /// <summary>
        ///     <see cref="PropertyDrawer.OnGUI" />
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            string propertyPath = property.propertyPath;
            Data data;
            if (!_data.TryGetValue(propertyPath, out data)) {
                data = new Data(property, label);
                _data[propertyPath] = data;
            }

            using (hGUI.Property(data.Content, position, property)) {
                data.UpdateContent(label);
                data.Draw(position, property, typeof(SceneAsset));
                _data[propertyPath] = data;
            }
        }

    }
}
