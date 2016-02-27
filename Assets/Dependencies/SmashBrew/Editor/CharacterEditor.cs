using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    [CustomEditor(typeof(Character))]
    internal class CharacterEditor : ScriptlessEditor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Damage", EditorStyles.boldLabel);
            var character = target as Character;
            GUI.enabled = character != null && EditorApplication.isPlayingOrWillChangePlaymode;
            character.CurrentDamage = EditorGUILayout.FloatField("Current Damage", character.CurrentDamage);
            character.DefaultDamage = EditorGUILayout.FloatField("Default Damage", character.DefaultDamage);
            GUI.enabled = true;
        }

    }
}
