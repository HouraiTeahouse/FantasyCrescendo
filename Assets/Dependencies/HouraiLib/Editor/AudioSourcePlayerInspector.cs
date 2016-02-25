using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    [CustomEditor(typeof(AudioSourceControl))]
    public class AudioSourcePlayerInspector : ScriptlessEditor {

        public override void OnInspectorGUI() {
            var source = (target as MonoBehaviour).GetComponent<AudioSource>();
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = EditorApplication.isPlayingOrWillChangePlaymode;
            if (GUILayout.Button("Play"))
                source.Play();
            if (GUILayout.Button("Pause"))
                source.Pause();
            if(GUILayout.Button("Stop"))
                source.Stop();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

    }
}
