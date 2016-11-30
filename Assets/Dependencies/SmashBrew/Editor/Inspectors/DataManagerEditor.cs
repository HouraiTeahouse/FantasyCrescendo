using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary> A custom Editor for DataManager </summary>
    [CustomEditor(typeof(DataManager))]
    public class DataManagerEditor : ScriptlessEditor {

        SerializedProperty characters;
        SerializedProperty scenes;

        void OnEnable() {
            characters = serializedObject.FindProperty("_characters");
            scenes = serializedObject.FindProperty("_scenes");
        }

        /// <summary>
        ///     <see cref="UnityEditor.Editor.OnInspectorGUI" />
        /// </summary>
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (GUILayout.Button("Refresh")) {
                characters.SetArray(Resources.LoadAll<CharacterData>(""));
                scenes.SetArray(Resources.LoadAll<SceneData>(""));
                serializedObject.ApplyModifiedProperties();
            }
        }

    }

}
