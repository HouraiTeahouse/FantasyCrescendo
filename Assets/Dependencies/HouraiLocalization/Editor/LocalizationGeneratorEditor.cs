using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Localization.Editor {

    /// <summary>
    /// Custom Editor for LocalizaitonGenerator.
    /// </summary>
    [CustomEditor(typeof (LocalizationGenerator))]
    public class LocalizationGeneratorEditor : UnityEditor.Editor {

        /// <summary>
        /// <see cref="UnityEditor.Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate")) {
                (target as LocalizationGenerator).Generate();
            }
        }

    }

}
