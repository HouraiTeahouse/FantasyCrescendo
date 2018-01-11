using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

    [CustomEditor(typeof(CharacterColor))]
    public class ColorStateEditor : Editor {

        const string kMaterialsName = "Materials";

        SerializedProperty swaps;

        CharacterColor CharacterColor => target as CharacterColor;
        
        int Pallete;

        bool RemoveButton => GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.MaxWidth(30));
        bool AddButton => GUILayout.Button("+", EditorStyles.toolbarButton);

        void OnEnable() { 
          swaps = serializedObject.FindProperty("Swaps"); 
        }

        void Delete(SerializedProperty property, int index) {
            // Must delete twice if it is not null.
            if (property.propertyType == SerializedPropertyType.ObjectReference
                && property.GetArrayElementAtIndex(index).objectReferenceValue != null)
                property.DeleteArrayElementAtIndex(index);
            property.DeleteArrayElementAtIndex(index);
        }

        void Add(SerializedProperty property) { 
          property.InsertArrayElementAtIndex(property.arraySize); 
        }

        void DrawArraySet(SerializedProperty array) {
          for (var i = 0; i < array.arraySize; i++) {
            using (new EditorGUILayout.HorizontalScope()) {
              EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), GUIContent.none);
              if (RemoveButton) {
                Delete(array, i);
              }
            }
          }
          if (AddButton) {
            Add(array);
          }
        }

        void DrawArraySet(SerializedProperty array, SerializedProperty sets) {
          for (var i = 0; i < array.arraySize; i++) {
            using (new EditorGUILayout.HorizontalScope()) {
              EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), GUIContent.none);
              if (RemoveButton) {
                for (var j = 0; j < sets.arraySize; j++)
                  Delete(sets.GetArrayElementAtIndex(j).FindPropertyRelative(kMaterialsName), i);
              }
            }
          }
          if (AddButton) {
            for (var j = 0; j < sets.arraySize; j++) {
              Add(sets.GetArrayElementAtIndex(j).FindPropertyRelative(kMaterialsName));
            }
          }
        }

        public override void OnInspectorGUI() {
          Pallete = EditorGUILayout.Popup("Current Color",
              Pallete, Enumerable.Range(0, CharacterColor.Count).Select(i => i.ToString()) .ToArray());
          EditorGUILayout.LabelField($"Swap: {Pallete}", EditorStyles.boldLabel);
          for (var i = 0; i < swaps.arraySize; i++) {
            SerializedProperty swap = swaps.GetArrayElementAtIndex(i);
            using (new EditorGUILayout.HorizontalScope()) {
              using (new EditorGUILayout.VerticalScope()) {
                DrawArraySet(swap.FindPropertyRelative("TargetRenderers"));
              }
              using (new EditorGUILayout.VerticalScope()) {
                SerializedProperty sets = swap.FindPropertyRelative("MaterialSets");
                if(sets.arraySize == 0) {
                  Add(sets);
                }
                DrawArraySet(sets.GetArrayElementAtIndex(Pallete).FindPropertyRelative(kMaterialsName), sets);
              }
              if (RemoveButton) {
                Delete(swaps, i);
              }
            }
          }
          if (AddButton) {
            Add(swaps);
          }
          using (new EditorGUILayout.HorizontalScope()) {
            if (GUILayout.Button("Add Swap")) {
              AddSwap();
            }
            if (CharacterColor.Count > 0) {
              if (GUILayout.Button("Remove Swap")) {
                RemoveSwap();
              }
            }
          }
          using (new EditorGUILayout.HorizontalScope()) {
            if (GUILayout.Button("Apply")) {
              CharacterColor.SetColor((uint)Pallete);
            }
            if (GUILayout.Button("Clear")) {
              CharacterColor.Clear();
            }
          }
          if (GUI.changed) {
            serializedObject.ApplyModifiedProperties();
          }
        }

        void AddSwap() {
            for (var i = 0; i < swaps.arraySize; i++) {
                SerializedProperty set = swaps.GetArrayElementAtIndex(i).FindPropertyRelative("MaterialSets");
                set.InsertArrayElementAtIndex(Pallete);
            }
            Pallete = Mathf.Clamp(Pallete + 1, 0, CharacterColor.Count);
        }

        void RemoveSwap() {
          for (var i = 0; i < swaps.arraySize; i++) {
            SerializedProperty set = swaps.GetArrayElementAtIndex(i).FindPropertyRelative("MaterialSets");
            set.DeleteArrayElementAtIndex(Pallete);
          }
          Pallete = Mathf.Clamp(Pallete - 1, 0, CharacterColor.Count);
        }

    }

}
