using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Hourai.Editor {

    /// <summary>
    /// Custom PropertyDrawer for ResourcePathAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(ResourceAttribute))]
    public class ResourcePathEditor : PropertyDrawer {

        private SerializedProperty prop;
        private Type type;
        private Object obj;
        private string message;
        private bool success;

        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            EditorGUI.BeginProperty(position, label, property);

            string path = property.stringValue;
            bool changed = prop != property;
            if (changed) {
                obj = Resources.Load(path);
                prop = property;
            }

            float height = base.GetPropertyHeight(property, label);
            position.height = height;

            label.text += " (" + ((success) ? "\u2713" : "\u2715") + ")"; 
            label.tooltip = message;

            obj = EditorGUI.ObjectField(position, label, obj, (attribute as ResourceAttribute).TypeRestriction, false);

            if (GUI.changed || changed) {
                message = string.Empty;
                success = false;
                if (!obj) {
                    path = string.Empty;
                    message = "No Object Specified";
                } else {
                    string resourcePath = Regex.Replace(AssetDatabase.GetAssetPath(obj), ".*/Resources/(.*?)\\..*", "$1");
                    if (path.Contains("/Resources/")) {
                        message = "Not in a Resources folder. Will not be saved.";
                    } else {
                        message = "Path: " + resourcePath;
                        path = resourcePath;
                        success = true;
                    }
                }
            }
            property.stringValue = path;

            EditorGUI.EndProperty();
        }

    }

}