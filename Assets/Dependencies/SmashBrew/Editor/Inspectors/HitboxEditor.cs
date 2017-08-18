using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace HouraiTeahouse.SmashBrew {

    [CustomEditor(typeof(Hitbox))]
    [CanEditMultipleObjects]
    public class HitboxEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var hitboxes = targets.OfType<Hitbox>();
            var hitboxType = hitboxes.Select(h => h.CurrentType).Distinct().FirstOrDefault();
            hitboxType = (Hitbox.Type)EditorGUILayout.EnumPopup("Type", hitboxType);
            foreach (var hitbox in hitboxes) {
                bool changed = hitbox.CurrentType != hitboxType;
                hitbox.CurrentType = hitboxType;
                if (changed)
                    EditorUtility.SetDirty(hitbox);
            }
            var isHitbox = serializedObject.FindProperty("_isHitbox");
            if (!isHitbox.boolValue)
                return;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_priority"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_damage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_angle"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_baseKnockback"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_knockbackScaling"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_reflectable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_absorbable"));
        }
    }

}