using System;
using UnityEngine;
using UnityEditor;

namespace UnityToolbag
{
    [CustomEditor(typeof(SortingLayerExposed))]
    public class SortingLayerExposedEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Get the renderer from the target object
            var renderer = (target as SortingLayerExposed).gameObject.GetComponent<Renderer>();

            // If there is no renderer, we can't do anything
            if (!renderer) {
                EditorGUILayout.HelpBox("SortingLayerExposed must be added to a game object that has a renderer.", MessageType.Error);
                return;
            }

            var sortingLayerNames = SortingLayerHelper.sortingLayerNames;

            // If we have the sorting layers array, we can make a nice dropdown. For stability's sake, if the array is null
            // we just use our old logic. This makes sure the script works in some fashion even if Unity changes the name of
            // that internal field we reflected.
            if (sortingLayerNames != null) {
                // Look up the layer name using the current layer ID
                string oldName = SortingLayerHelper.GetSortingLayerNameFromID(renderer.sortingLayerID);

                // Use the name to look up our array index into the names list
                int oldLayerIndex = Array.IndexOf(sortingLayerNames, oldName);

                // Show the popup for the names
                int newLayerIndex = EditorGUILayout.Popup("Sorting Layer", oldLayerIndex, sortingLayerNames);

                // If the index changes, look up the ID for the new index to store as the new ID
                if (newLayerIndex != oldLayerIndex) {
                    Undo.RecordObject(renderer, "Edit Sorting Layer");
                    renderer.sortingLayerID = SortingLayerHelper.GetSortingLayerIDForIndex(newLayerIndex);
                    EditorUtility.SetDirty(renderer);
                }
            }
            else {
                // Expose the sorting layer name
                string newSortingLayerName = EditorGUILayout.TextField("Sorting Layer Name", renderer.sortingLayerName);
                if (newSortingLayerName != renderer.sortingLayerName) {
                    Undo.RecordObject(renderer, "Edit Sorting Layer Name");
                    renderer.sortingLayerName = newSortingLayerName;
                    EditorUtility.SetDirty(renderer);
                }

                // Expose the sorting layer ID
                int newSortingLayerId = EditorGUILayout.IntField("Sorting Layer ID", renderer.sortingLayerID);
                if (newSortingLayerId != renderer.sortingLayerID) {
                    Undo.RecordObject(renderer, "Edit Sorting Layer ID");
                    renderer.sortingLayerID = newSortingLayerId;
                    EditorUtility.SetDirty(renderer);
                }
            }

            // Expose the manual sorting order
            int newSortingLayerOrder = EditorGUILayout.IntField("Sorting Layer Order", renderer.sortingOrder);
            if (newSortingLayerOrder != renderer.sortingOrder) {
                Undo.RecordObject(renderer, "Edit Sorting Order");
                renderer.sortingOrder = newSortingLayerOrder;
                EditorUtility.SetDirty(renderer);
            }
        }
    }
}
