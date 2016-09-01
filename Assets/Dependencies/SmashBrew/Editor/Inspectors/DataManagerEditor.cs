using System;
using System.Linq;
using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary>
    /// A custom Editor for DataManager
    /// </summary>
    [CustomEditor(typeof(DataManager))]
    public class DataManagerEditor : ScriptlessEditor {

        SerializedProperty characters;
        SerializedProperty scenes;
        ReorderableList characterList;
        ReorderableList sceneList;

        /// <summary>
        /// <see cref="UnityEditor.Editor.OnEnable"/>
        /// </summary>
        void OnEnable() {
            characters = serializedObject.FindProperty("_characters");
            scenes = serializedObject.FindProperty("_scenes");
            characterList = new ReorderableList(serializedObject, characters, true, false, true, true);
            sceneList = new ReorderableList(serializedObject, scenes, true, false, true, true);
            InitializeList(characterList);
            InitializeList(sceneList);
        }

        void InitializeList(ReorderableList list) {
            list.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused) {
                rect.y += 2;
                rect.height -= 4;
                EditorGUI.PropertyField(rect, list.serializedProperty.GetArrayElementAtIndex(index), new GUIContent(string.Format("ID: {0}", index.ToString("D3"))));
            };
            list.drawHeaderCallback = delegate(Rect rect) {
                EditorGUI.LabelField(rect, list.serializedProperty.displayName);
            };
            list.onAddCallback = delegate(ReorderableList reorderableList) {
                SerializedProperty property = reorderableList.serializedProperty;
                property.InsertArrayElementAtIndex(property.arraySize);
            };
        }

        /// <summary>
        /// <see cref="UnityEditor.Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dontDestroyOnLoad"));
            characterList.DoLayoutList();
            sceneList.DoLayoutList();
            if (GUILayout.Button("Refresh")) {
                characters.SetArray(Resources.LoadAll<CharacterData>(""));
                scenes.SetArray(Resources.LoadAll<SceneData>(""));
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
