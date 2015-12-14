using UnityEditor;
using UnityEngine;

namespace Hourai.Localization.Editor {

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