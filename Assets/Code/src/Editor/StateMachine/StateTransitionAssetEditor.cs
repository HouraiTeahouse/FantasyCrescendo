using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[CustomEditor(typeof(StateTransitionAsset))]
public class StateTransitionAssetEditor : Editor {

  ReorderableList _conditionList;
  GUIStyle _enabledToggle;

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {
    _conditionList = new ReorderableList(
      serializedObject,
      elements: serializedObject.FindProperty("_conditions"),
      draggable: true,
      displayHeader: true,
      displayAddButton: true,
      displayRemoveButton: true
    );
    _conditionList.drawHeaderCallback = OnConditionHeaderGUI;
    _conditionList.drawElementCallback = OnDrawConditionGUI;

    _enabledToggle = new GUIStyle(EditorStyles.toolbarButton);
    _enabledToggle.normal.background = _enabledToggle.active.background;
  }

  public override void OnInspectorGUI() {
    EditorGUI.BeginChangeCheck();
    EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionRequirement"));
    EditorGUILayout.Space();
    _conditionList.DoLayoutList();
    if (EditorGUI.EndChangeCheck()) {
      serializedObject.ApplyModifiedProperties();
    }
  }

  protected sealed override void OnHeaderGUI() {
    var transition = target as StateTransitionAsset;
    var name = serializedObject.FindProperty("m_Name");
    var muted = serializedObject.FindProperty("Muted");

    EditorGUI.BeginChangeCheck();
    EditorGUILayout.BeginVertical(EditorStyles.toolbar);
    EditorGUILayout.BeginHorizontal();
    var nameText = EditorGUILayout.TextField(transition.DisplayName, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
    name.stringValue = nameText != transition.DefaultName ? nameText : string.Empty;
    if (GUILayout.Button("Muted", muted.boolValue ? _enabledToggle : EditorStyles.toolbarButton, GUILayout.MaxWidth(50))) {
      muted.boolValue = !muted.boolValue;
    }
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.EndVertical();
    if (EditorGUI.EndChangeCheck()) {
      serializedObject.ApplyModifiedProperties();
    }
  }

  void OnConditionHeaderGUI(Rect rect) => EditorGUI.LabelField(rect, "Conditions");

  void OnDrawConditionGUI(Rect rect, int index, bool isActive, bool isFocused) {
    var element = _conditionList.serializedProperty.GetArrayElementAtIndex(index);
    var typeProperty = element.FindPropertyRelative("Type");
    EditorGUI.PropertyField(GetSubRect(rect, 0f, 1/3f), typeProperty, GUIContent.none);
    switch (GetConditionType(typeProperty)) {
      case StateTransitionCondition.ConditionType.InputDirection: 
        var axis = element.FindPropertyRelative("Axis");
        var direction = element.FindPropertyRelative("AxisDirection");
        EditorGUI.PropertyField(GetSubRect(rect, 1/3f, 1/3f), axis, GUIContent.none);
        EditorGUI.PropertyField(GetSubRect(rect, 2/3f, 1/3f), direction, GUIContent.none);
        break;
      default:
        var subproperty = GetConditionSubProperty(element, typeProperty);
        EditorGUI.PropertyField(GetSubRect(rect, 1/3f, 2/3f), subproperty, GUIContent.none);
        break;
    }
  }

  StateTransitionCondition.ConditionType GetConditionType(SerializedProperty property) {
    return (StateTransitionCondition.ConditionType)property.intValue;
  }

  SerializedProperty GetConditionSubProperty(SerializedProperty root, SerializedProperty type) {
    switch (GetConditionType(type)) {
      case StateTransitionCondition.ConditionType.InputButtonWasPressed: 
      case StateTransitionCondition.ConditionType.InputButtonWasReleased: 
      case StateTransitionCondition.ConditionType.InputButtonHeld:
        return root.FindPropertyRelative("Button");
      case StateTransitionCondition.ConditionType.InputDirection: 
        return root.FindPropertyRelative("Axis");
      case StateTransitionCondition.ConditionType.IsGrounded:
      case StateTransitionCondition. ConditionType.IsHit:
      case StateTransitionCondition.ConditionType.IsGrabbingLedge:
      case StateTransitionCondition.ConditionType.CanJump:
      case StateTransitionCondition.ConditionType.IsShieldBroken:
      case  StateTransitionCondition.ConditionType.PlayerDirection:
        return root.FindPropertyRelative("BoolParam");
      case StateTransitionCondition.ConditionType.Time:
        return root.FindPropertyRelative("FloatParam");
      default: return null;
    }
  }

  Rect GetSubRect(Rect original, float start, float length) {
    original.x += start * original.width;
    original.width = length * original.width;
    return original;
  }

}

}
