using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
using UnityObject = UnityEngine.Object;

namespace Genso.API.Editor {

    [CustomPropertyDrawer(typeof(ResourcePathAttribute))]
    public class ResourcePathAttributeDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            //Target cannot be written to as a path, ignore completely
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            Type targetType = (attribute as ResourcePathAttribute).TargetType;
            label.text += targetType == null ? "" : " (" + targetType.Name + ")";
            EditorGUI.PropertyField(position, property, label);

            Event currentEvent = Event.current;
            EventType currentEventType = currentEvent.type;
            if ( currentEventType == EventType.DragExited ) 
                DragAndDrop.PrepareStartDrag();// Clear generic data when user pressed escape. (Unfortunately, DragExited is also called when the mouse leaves the drag area)
 
            if (!position.Contains(currentEvent.mousePosition))
                return;

            switch (currentEventType){
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    UnityObject validTarget = null;
                    foreach (var target in DragAndDrop.objectReferences)
                        if (targetType.IsInstanceOfType(target))
                            validTarget = target;

                    if (currentEventType == EventType.DragUpdated)
                        DragAndDrop.visualMode = validTarget != null
                                                     ? DragAndDropVisualMode.Link
                                                     : DragAndDropVisualMode.Rejected;
                    else {
                        DragAndDrop.AcceptDrag();
                        string path = AssetDatabase.GetAssetPath(validTarget);
                        if (!path.Contains("Resources")) {
                            Debug.LogError("The object supplied was the right type, but was not located inside a Resources folder. Please move it to a Resources folder.");
                            property.stringValue = "";
                        }
                        else
                            property.stringValue = Regex.Replace(path, ".*Resources/(.*)\\..*", "$1");
                    }
                currentEvent.Use();
                break;
            case EventType.MouseUp:
                // Clean up, in case MouseDrag never occurred:
                DragAndDrop.PrepareStartDrag();
                break;
            }
   
        }

    }

}
