using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary>
    /// Custom Editor for CharacterStateEvents
    /// </summary>
    [CustomEditor(typeof(CharacterStateEvents))]
    internal class CharacterStateEventsEditor : UnityEditor.Editor {

        SerializedProperty clip;

        bool Initialized {
            get { return clip.objectReferenceValue != null; }
        }

        void OnEnable() {
            clip = serializedObject.FindProperty("_clip");
            if (!Initialized)
                Initialize();
        }

        void Initialize() {
            var state = Selection.objects
                    .OfType<AnimatorState>()
                    .FirstOrDefault(s => s.behaviours.Contains(target));
            if (state == null)
                return;
            clip.objectReferenceValue = state.motion as AnimationClip;
            serializedObject.ApplyModifiedProperties();
            Repaint();
            Log.Info("Initalize");
        }

        public override void OnInspectorGUI() {
            if (!Initialized) {
                EditorGUILayout.HelpBox("No animation clip found. Please supply state with an AnimationClip and initialize.", MessageType.Error);
                if(GUILayout.Button("Initialize"))
                    Initialize();
            } else {
                if (!GUILayout.Button("Open Events Editor")) {
                    var window = EditorWindow.GetWindow<EventsEditorWindow>();
                    window.Show();
                }
            }
        }
    }

}

