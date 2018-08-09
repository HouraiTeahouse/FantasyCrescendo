using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[CustomEditor(typeof(BaseStateAsset), true)]
[CanEditMultipleObjects]
public class BaseStateAssetEditor : Editor {

  public override void OnInspectorGUI() {
    // EditorGUILayout.PropertyField(serializedObject.FindProperty("StateType"));
    EditorGUI.BeginChangeCheck();
    OnStateGUI();
    OnTransitionGUI();
    if (EditorGUI.EndChangeCheck()) {
      serializedObject.ApplyModifiedProperties();
    }
  }

  protected virtual void OnStateGUI() { }

  void OnTransitionGUI() {
    EditorGUILayout.Space();
    EditorGUILayout.LabelField("Transitions", EditorStyles.boldLabel);
    EditorGUILayout.PropertyField(serializedObject.FindProperty("_transitions"), true);
  }

  protected sealed override void OnHeaderGUI() {
    EditorGUILayout.BeginVertical(GUI.skin.GetStyle("IN GameObjectHeader"));
    EditorGUI.BeginChangeCheck();
    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Name"), GUIContent.none);
    OnCustomHeaderGUI();
    if (EditorGUI.EndChangeCheck()) {
      serializedObject.ApplyModifiedProperties();
    }
    EditorGUILayout.EndVertical();
    EditorGUILayout.Space();
  }

  protected virtual void OnCustomHeaderGUI() { }

}

}
