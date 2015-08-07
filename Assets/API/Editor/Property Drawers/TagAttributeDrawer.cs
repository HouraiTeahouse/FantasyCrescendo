using UnityEngine;
using UnityEditor;

namespace Crescendo.API.Editor
{

    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Make sure the target property is a string
            if (property.propertyType != SerializedPropertyType.String)
                EditorGUI.PropertyField(position, property);


            EditorGUI.BeginProperty(position, label, property);

            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);

            EditorGUI.EndProperty();
        }

    }


}