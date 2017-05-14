using UnityEditor;

namespace HouraiTeahouse.SmashBrew.Characters {


    [CustomEditor(typeof(Character))]
    public class CharacterEditor : UnityEditor.Editor  {

        public override void OnInspectorGUI() {
            if (EditorApplication.isPlayingOrWillChangePlaymode) {
                var state = string.Empty;
                var controller = (target as Character).StateController;
                if (controller != null && controller.CurrentState != null)
                    state = controller.CurrentState.Name;
                EditorGUILayout.LabelField("Current State", state);
            }
            DrawDefaultInspector();
        }

    }

}