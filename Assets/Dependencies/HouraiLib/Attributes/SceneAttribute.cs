using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse {
    /// <summary>
    /// PropertyAttribute with a drawer that exposes a SceneAsset object field.
    /// MUST be a string field.
    /// Saves the path of the SceneAsset to the field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneAttribute : PropertyAttribute {
    }

#if UNITY_EDITOR
    /// <summary>
    /// Custom PropertyDrawer for SceneAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof (SceneAttribute))]
    internal class SceneAttributeDrawer : PropertyDrawer {

        private readonly Dictionary<SerializedProperty, SceneAsset> _scenes;

        public SceneAttributeDrawer() {
            _scenes = new Dictionary<SerializedProperty, SceneAsset>();
        }

        /// <summary>
        /// <see cref="PropertyDrawer.OnGUI"/>
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                base.OnGUI(position, property, label);
                return;
            }
            if(!_scenes.ContainsKey(property))
                _scenes[property] = AssetDatabase.LoadAssetAtPath<SceneAsset>(string.Format("Assets/{0}.unity", property.stringValue));
            EditorGUI.BeginChangeCheck();
            _scenes[property] = EditorGUI.ObjectField(position, label, _scenes[property], typeof(SceneAsset), false) as SceneAsset;
            if (EditorGUI.EndChangeCheck())
                property.stringValue = Regex.Replace(AssetDatabase.GetAssetPath(_scenes[property]), "Assets/(.*)\\.unity", "$1");
        }
    }
#endif
}
