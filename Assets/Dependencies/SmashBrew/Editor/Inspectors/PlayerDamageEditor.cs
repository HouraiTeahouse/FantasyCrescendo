using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary> A custom Editor for Character </summary>
    [CustomEditor(typeof(PlayerDamage))]
    internal class PlayerDamageEditor : ScriptlessEditor {

        /// <summary>
        ///     <see cref="Editor.OnInspectorGUI" />
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var character = target as PlayerDamage;
            GUI.enabled = character != null && EditorApplication.isPlayingOrWillChangePlaymode;
            character.CurrentDamage = EditorGUILayout.FloatField("Current Damage", character.CurrentDamage);
            character.DefaultDamage = EditorGUILayout.FloatField("Default Damage", character.DefaultDamage);
            GUI.enabled = true;
        }

    }

}