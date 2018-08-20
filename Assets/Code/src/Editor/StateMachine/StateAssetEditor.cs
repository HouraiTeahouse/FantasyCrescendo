using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[CustomEditor(typeof(StateAsset))]
public class StateAssetEditor : BaseStateAssetEditor {

  protected override void OnStateGUI() {
    EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
    OnDataGUI();
  }

  protected override void OnCustomHeaderGUI() {
    EditorGUILayout.PropertyField(serializedObject.FindProperty("_stateType"));
  }

  void OnDataGUI() {
    var baseProperty = serializedObject.FindProperty("StateData");
    foreach (var obj in baseProperty) {
      var property = obj as SerializedProperty;
      if (property != null) {
        EditorGUILayout.PropertyField(property);
      }
    }
  }

}

}