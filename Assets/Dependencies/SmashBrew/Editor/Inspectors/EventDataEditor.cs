using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    [CustomEditor(typeof(EventData))]
    internal class EventDataEditor : ScriptlessEditor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open Event Editor"))
                EventsEditorWindow.GetWindow().Show();
        }

    }

}