using UnityEditor;
using UnityEngine;

namespace Hourai.SmashBrew.Editor {

    [CustomEditor(typeof(SceneData))]
    public class SceneDataEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Load")) {
                (target as SceneData).Load();
            }
        }

    }

}