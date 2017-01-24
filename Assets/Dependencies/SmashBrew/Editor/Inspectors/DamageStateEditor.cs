using System.Collections;
using System.Collections.Generic;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    [CustomEditor(typeof(DamageState))]
    public class DamageStateEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var damage = target as DamageState;
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            damage.CurrentDamage = EditorGUILayout.FloatField("Current Damage", damage.CurrentDamage);
        }

    }

}

