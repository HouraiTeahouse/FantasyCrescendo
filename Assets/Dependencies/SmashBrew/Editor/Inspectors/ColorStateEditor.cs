using System.Linq;
using HouraiTeahouse.Editor;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    [CustomEditor(typeof(ColorState))]
    public class ColorStateEditor : ScriptlessEditor {

        SerializedProperty _swaps;

        bool _simulate;
        int _swapIndex = 0;
        
        int SwapIndex {
            get { return _swapIndex; }
            set {
                _swapIndex = value;
                UpdateMesh();
            }
        }

        void UpdateMesh() {
            if (_simulate)
                Swap.Pallete = SwapIndex;
            else
                Swap.ResetSwaps();
        }

        ColorState Swap {
            get { return target as ColorState; }
        }

        bool RemoveButton {
            get { return GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.MaxWidth(30)); }
        }

        bool AddButton {
            get { return GUILayout.Button("+", EditorStyles.toolbarButton); }
        }

        void OnEnable() { _swaps = serializedObject.FindProperty("_swaps"); }

        void Delete(SerializedProperty property, int index) {
            // Must delete twice if it is not null.
            if (property.propertyType == SerializedPropertyType.ObjectReference
                && property.GetArrayElementAtIndex(index).objectReferenceValue != null)
                property.DeleteArrayElementAtIndex(index);
            property.DeleteArrayElementAtIndex(index);
        }

        void Add(SerializedProperty property) { property.InsertArrayElementAtIndex(property.arraySize); }

        void DrawArraySet(SerializedProperty array) {
            for (var i = 0; i < array.arraySize; i++) {
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), GUIContent.none);
                    if (RemoveButton)
                        Delete(array, i);
                }
            }
            if (AddButton)
                Add(array);
        }

        void DrawArraySet(SerializedProperty array, SerializedProperty sets) {
            for (var i = 0; i < array.arraySize; i++) {
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), GUIContent.none);
                    if (RemoveButton)
                        for (var j = 0; j < sets.arraySize; j++)
                            Delete(sets.GetArrayElementAtIndex(j).FindPropertyRelative("_materials"), i);
                }
            }
            if (AddButton)
                for (var j = 0; j < sets.arraySize; j++)
                    Add(sets.GetArrayElementAtIndex(j).FindPropertyRelative("_materials"));
        }

        public override void OnInspectorGUI() {
            if (Check.Range(SwapIndex, Swap.Count))
                SwapIndex = Mathf.Clamp(SwapIndex, 0, Swap.Count);
            SwapIndex = EditorGUILayout.Popup("Current Color",
                SwapIndex,
                Enumerable.Range(0, Swap.Count)
                    .Select(i => i.ToString())
                    .ToArray());
            EditorGUILayout.LabelField(string.Format("Swap: {0}", SwapIndex), EditorStyles.boldLabel);
            for (var i = 0; i < _swaps.arraySize; i++) {
                SerializedProperty swap = _swaps.GetArrayElementAtIndex(i);
                using (new EditorGUILayout.HorizontalScope()) {
                    using (new EditorGUILayout.VerticalScope()) {
                        DrawArraySet(swap.FindPropertyRelative("TargetRenderers"));
                    }
                    using (new EditorGUILayout.VerticalScope()) {
                        SerializedProperty sets = swap.FindPropertyRelative("MaterialSets");
                        if(sets.arraySize == 0)
                            Add(sets);
                        DrawArraySet(sets.GetArrayElementAtIndex(SwapIndex).FindPropertyRelative("_materials"), sets);
                    }
                    if (RemoveButton)
                        Delete(_swaps, i);
                }
            }
            if (AddButton)
                Add(_swaps);
            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Add Swap"))
                    AddSwap();
                if (Swap.Count > 0)
                    if (GUILayout.Button("Remove Swap"))
                        RemoveSwap();
            }
            _simulate = EditorGUILayout.Toggle("Simulate", _simulate);
            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
                UpdateMesh();
            }
        }

        void AddSwap() {
            for (var i = 0; i < _swaps.arraySize; i++) {
                SerializedProperty set = _swaps.GetArrayElementAtIndex(i).FindPropertyRelative("MaterialSets");
                set.InsertArrayElementAtIndex(SwapIndex);
            }
            SwapIndex++;
        }

        void RemoveSwap() {
            for (var i = 0; i < _swaps.arraySize; i++) {
                SerializedProperty set = _swaps.GetArrayElementAtIndex(i).FindPropertyRelative("MaterialSets");
                set.DeleteArrayElementAtIndex(SwapIndex);
            }
            SwapIndex--;
        }

    }

}
