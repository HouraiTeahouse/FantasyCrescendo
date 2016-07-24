using System.Linq;
using HouraiTeahouse.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary>
    /// Custom Editor for CharacterStateEvents
    /// </summary>
    [CustomEditor(typeof(CharacterStateEvents))]
    internal class CharacterStateEventsEditor : UnityEditor.Editor {

        AnimatorState state;

        bool Initialized {
            get { return state != null && 
                    Clip.objectReferenceValue != null && 
                    Clip.objectReferenceValue == state.motion as AnimationClip && 
                    Name.stringValue == state.name; }
        }

        SerializedProperty Clip {
            get { return serializedObject.FindProperty("_clip"); }
        }

        SerializedProperty Name {
            get { return serializedObject.FindProperty("_stateName"); }
        }

        SerializedProperty EventData {
            get { return serializedObject.FindProperty("_eventData"); }
        }

        void OnEnable() {
            Selection.selectionChanged += CheckInitialize;
            CheckInitialize();
        }

        void OnDiable() { Selection.selectionChanged -= CheckInitialize; }

        void CheckInitialize() {
            state = Selection.objects
                    .OfType<AnimatorState>()
                    .FirstOrDefault(s => s.behaviours.Contains(target));
            if (!Initialized)
                Initialize();
        }

        void Initialize() {
            if (state == null)
                return;
            Clip.objectReferenceValue = state.motion as AnimationClip;
            Name.stringValue = state.name;
            serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        public override void OnInspectorGUI() {
            if (!Initialized) {
                EditorGUILayout.HelpBox("No animation clip found. Please supply state with an AnimationClip and initialize.", MessageType.Error);
                if(GUILayout.Button("Initialize"))
                    Initialize();
                return;
            }
            EditorGUILayout.PropertyField(EventData);
            if (EventData.objectReferenceValue == null) {
                if(GUILayout.Button("Create Event Data")) {
                    var eventData = CreateInstance<EventData>();
                    eventData.name = state == null
                        ? "New Event Data"
                        : state.name;
                    AssetUtil.CreateAsset(string.Empty, eventData);
                    EventData.objectReferenceValue = eventData;
                    serializedObject.ApplyModifiedProperties();
                }
                return;
            }
            if (GUILayout.Button("Open Behaviour Editor")) {
                var window = EditorWindow.GetWindow<EventsEditorWindow>();
                window.Show();
            }
        }
    }
}

