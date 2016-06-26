using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary>
    /// Custom Editor for CharacterStateEvents
    /// </summary>
    [CustomEditor(typeof(CharacterStateEvents))]
    internal class CharacterStateEventsEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            if(serializedObject.FindProperty("_clip").objectReferenceValue == null)
                EditorGUILayout.HelpBox("No animation clip found. Please supply state with an AnimationClip and initialize.", MessageType.Error);
            if (!GUILayout.Button("Open Events Editor"))
                return;
            var window = EditorWindow.GetWindow<EventsEditorWindow>();
            window.Show();
        }
    }

}

