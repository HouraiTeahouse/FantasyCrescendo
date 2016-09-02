using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary> A custom Editor for SceneData </summary>
    [CustomEditor(typeof(SceneData))]
    public class SceneDataEditor : ScriptlessEditor {

        /// <summary>
        ///     <see cref="UnityEditor.Editor.OnInspectorGUI" />
        /// </summary>
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            string message;
            MessageType type;
            if (!Assets.IsResource(target)) {
                message =
                    "This game cannot find this Scene Data if it is not in a Resources folder. Please move it to a Resources (sub)folder.";
                type = MessageType.Error;
            }
            else {
                message = "This Scene Data is correctly placed. The game can find it.";
                type = MessageType.Info;
            }
            EditorGUILayout.HelpBox(message, type);
            if (GUILayout.Button("Load")) {
                (target as SceneData).Load();
            }
        }

    }

}