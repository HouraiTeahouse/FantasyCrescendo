using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Custom Editor for TimeModifier.
    /// </summary>
    [CustomEditor(typeof(TimeModifier))]
    internal class TimeModifierEditor : ScriptlessEditor {

        /// <summary>
        /// <see cref="UnityEditor.Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var modifier = target as TimeModifier;
            GUI.enabled = modifier != null && !EditorApplication.isPlayingOrWillChangePlaymode;
            modifier.LocalTimeScale = EditorGUILayout.FloatField("Time Scale", modifier.LocalTimeScale);
            GUI.enabled = true;
        }

    }
}
