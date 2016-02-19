using System.Linq;
using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    [CustomEditor(typeof(DataManager))]
    public class DataManagerEditor : ScriptlessEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Refresh")) {
                serializedObject.FindProperty("_characters")
                    .SetArray(Resources.LoadAll<CharacterData>(""));
                serializedObject.FindProperty("_scenes")
                    .SetArray(Resources.LoadAll<SceneData>(""));
                serializedObject.ApplyModifiedProperties();
            }

        }
    }
}
