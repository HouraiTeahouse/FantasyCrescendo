using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary> Custom Editor for AudioSourceControl. </summary>
    [CustomEditor(typeof(AudioSourceControl))]
    internal class AudioSourceControlInspector : ScriptlessEditor {

        /// <summary>
        ///     <see cref="UnityEditor.Editor.OnInspectorGUI" />
        /// </summary>
        public override void OnInspectorGUI() {
            var source = (target as MonoBehaviour).GetComponent<AudioSource>();
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = EditorApplication.isPlayingOrWillChangePlaymode;
            if (GUILayout.Button("Play"))
                source.Play();
            if (GUILayout.Button("Pause"))
                source.Pause();
            if (GUILayout.Button("Stop"))
                source.Stop();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

    }

}