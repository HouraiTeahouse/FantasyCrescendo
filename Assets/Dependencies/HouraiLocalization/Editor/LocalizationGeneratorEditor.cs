using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Localization.Editor {

    [CustomEditor(typeof (LocalizationGenerator))]
    public class LocalizationGeneratorEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate")) {
                (target as LocalizationGenerator).Generate();
            }
        }

    }

}
