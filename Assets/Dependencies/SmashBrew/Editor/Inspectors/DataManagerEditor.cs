using System.Linq;
using HouraiTeahouse.Editor;
using UnityEditor;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary> A custom Editor for DataManager </summary>
    [CustomEditor(typeof(DataManager))]
    public class DataManagerEditor : ScriptlessEditor {

        bool charactersFoldout;
        bool scenesFoldout;

        /// <summary>
        ///     <see cref="UnityEditor.Editor.OnInspectorGUI" />
        /// </summary>
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (!EditorApplication.isPlayingOrWillChangePlaymode || DataManager.Instance == null)
                return;
            charactersFoldout = EditorGUILayout.Foldout(charactersFoldout, "Loaded Characters");
            if (charactersFoldout) {
                EditorGUI.indentLevel++;
                foreach (var character in DataManager.Instance.Characters.OrderBy(c => c.FullName))
                    EditorGUILayout.LabelField(character.FullName, character.Id.ToString());
                EditorGUI.indentLevel--;
            }
            scenesFoldout = EditorGUILayout.Foldout(scenesFoldout, "Loaded Scenes");
            if (scenesFoldout) {
                EditorGUI.indentLevel++;
                foreach (var scene in DataManager.Instance.Scenes.OrderBy(c => c.name))
                    EditorGUILayout.LabelField(scene.Name, scene.Type.ToString());
                EditorGUI.indentLevel--;
            }
        }

    }

}
