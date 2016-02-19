using UnityEngine;
using System.Collections;
using UnityEditor;

namespace HouraiTeahouse.Editor {

    [CustomEditor(typeof(UniqueObject))]
    public class UniqueObjectEditor : ScriptlessEditor {
        public override void OnInspectorGUI() {
            var uniqueObject = target as UniqueObject;
            EditorGUILayout.LabelField("ID", uniqueObject.ID);
        }
    }
}
