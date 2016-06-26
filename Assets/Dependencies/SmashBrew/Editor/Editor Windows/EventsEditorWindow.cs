using UnityEngine;
using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEditor.Animations;

namespace HouraiTeahouse.SmashBrew.Editor {

    public class EventsEditorWindow : LockableEditorWindow {

        public AnimatorState State { get; set; }

        float SeekTime { get; set; }

        [MenuItem("Window/Animator Events Editor")]
        static void CreateWindow() {
            GetWindow<EventsEditorWindow>("Events").Show();
        }

        void OnSelectionChange() {
            if (IsLocked)
                return;
            var newState = Selection.activeObject as AnimatorState;
            if (newState == State)
                return;
            State = Selection.activeObject as AnimatorState;
            SeekTime = 0f;
            Repaint();
        }

        void OnGUI() {
            if (State == null)
                return;
            SeekTime = EditorGUILayout.Slider(GUIContent.none, SeekTime, 0f, 1f);
            foreach(StateMachineBehaviour behaviour in State.behaviours)
                EditorGUILayout.LabelField(behaviour.ToString());
        }

    }

}

