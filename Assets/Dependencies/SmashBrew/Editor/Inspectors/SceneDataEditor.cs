using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> A custom Editor for SceneData </summary>
    [CustomEditor(typeof(SceneData))]
    public class SceneDataEditor : ScriptlessEditor {

        /// <summary>
        ///     <see cref="UnityEditor.Editor.OnInspectorGUI" />
        /// </summary>
        public override void OnInspectorGUI() {
            if (!Assets.IsResource(target) && !Assets.IsBundleAsset(target))
                EditorGUILayout.HelpBox( "Scene Data is not bundled nor in a Resource folder. The game cannot find it.", MessageType.Error);
            DrawDefaultInspector();
            if (GUILayout.Button("Load")) {
                (target as SceneData).Load();
            }
        }

    }

}
