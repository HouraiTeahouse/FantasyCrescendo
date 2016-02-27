using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Custom PropertyDrawer for SceneAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof (SceneAttribute))]
    internal class SceneAttributeDrawer : PropertyDrawer {

        private Dictionary<SerializedProperty, SceneAsset> _scenes; 

        /// <summary>
        /// <see cref="PropertyDrawer.OnGUI"/>
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                base.OnGUI(position, property, label);
                return;
            }
            if(_scenes == null)
                _scenes = new Dictionary<SerializedProperty, SceneAsset>();

            if(!_scenes.ContainsKey(property))
                _scenes[property] = AssetDatabase.LoadAssetAtPath<SceneAsset>(string.Format("Assets/{0}.unity", property.stringValue));

            EditorGUI.BeginChangeCheck();
            _scenes[property] = EditorGUI.ObjectField(position, label, _scenes[property], typeof(SceneAsset), false) as SceneAsset;
            if (EditorGUI.EndChangeCheck())
                property.stringValue = Regex.Replace(AssetDatabase.GetAssetPath(_scenes[property]), "Assets/(.*)\\.unity", "$1");
        }

    }

}
