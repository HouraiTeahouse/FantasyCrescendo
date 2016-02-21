using UnityEditor;

namespace HouraiTeahouse.Editor {

    [CustomEditor(typeof(TimeModifier))]
    public class TimeModifierEditor : ScriptlessEditor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var damage = target as TimeModifier;
            if (damage == null || !EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            damage.LocalTimeScale = EditorGUILayout.FloatField("Time Scale", damage.LocalTimeScale);
        }

    }
}
