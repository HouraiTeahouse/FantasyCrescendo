using HouraiTeahouse.Editor;
using UnityEditor;

namespace HouraiTeahouse.SmashBrew.Editor {

    [CustomEditor(typeof(Damage))]
    public class DamageEditor : ScriptlessEditor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var damage = target as Damage;
            if (damage == null || !EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            damage.CurrentDamage = EditorGUILayout.FloatField("Current Damage", damage.CurrentDamage);
            damage.DefaultDamage = EditorGUILayout.FloatField("Default Damage", damage.DefaultDamage);
        }

    }
}
