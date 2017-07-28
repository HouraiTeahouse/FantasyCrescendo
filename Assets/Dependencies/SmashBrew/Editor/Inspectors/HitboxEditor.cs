using HouraiTeahouse.Editor;
using UnityEditor;

namespace HouraiTeahouse.SmashBrew.Editor {

    [CustomEditor(typeof(Hitbox))]
    public class HitboxEditor : BaseEditor<Hitbox> {

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Current Type", Target.CurrentType.ToString());
            base.OnInspectorGUI();
        }

        public override bool RequiresConstantRepaint() { return true; }

    }

}