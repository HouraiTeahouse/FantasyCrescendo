using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[CustomEditor(typeof(BaseStateAsset), true)]
[CanEditMultipleObjects]
public class BaseStateAssetEditor : Editor {

  const float kMuteButtonWidth = 50f;

  ReorderableList _transitionList;
  Dictionary<Object, Editor> _transitionEditors;

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  protected virtual void OnEnable() {
    _transitionEditors = new Dictionary<Object, Editor>();

    _transitionList = new ReorderableList(
      serializedObject,
      elements: serializedObject.FindProperty("_transitions"),
      draggable: true,
      displayHeader: true,
      displayAddButton: false,
      displayRemoveButton: true
    );
    _transitionList.drawHeaderCallback = OnDrawTransitionHeader;
    _transitionList.drawElementCallback = OnDrawTransitionGUI;
    _transitionList.onRemoveCallback = OnRemoveTransition;
  }

  public override void OnInspectorGUI() {
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
    _transitionList.DoLayoutList();
    EditorGUILayout.Space();
    if (_transitionList.count <= 0) return;
    if (_transitionList.index < 0) _transitionList.index = 0;
    var property = GetSelectedProperty(_transitionList);
    var editor = GetEditor(property.objectReferenceValue);
    editor.DrawHeader();
    EditorGUILayout.Space();
    editor.OnInspectorGUI();
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

  void OnDrawTransitionHeader(Rect rect) {
    rect.width -= kMuteButtonWidth;
    EditorGUI.LabelField(rect, "Transitions");
    rect.x += rect.width;
    rect.width = kMuteButtonWidth;
    EditorGUI.LabelField(rect, "Muted");
  }

  void OnDrawTransitionGUI(Rect rect, int index, bool isActive, bool isFocused) {
    var element = _transitionList.serializedProperty.GetArrayElementAtIndex(index);
    var editor = GetEditor(element.objectReferenceValue);
    var transition = element.objectReferenceValue as StateTransitionAsset;
    rect.width -= kMuteButtonWidth;
    EditorGUI.LabelField(rect, transition.DisplayName);
    rect.x += rect.width;
    rect.width = kMuteButtonWidth;
    EditorGUI.BeginChangeCheck();
    EditorGUI.PropertyField(rect, editor.serializedObject.FindProperty("Muted"), GUIContent.none);
    if (EditorGUI.EndChangeCheck()) {
      editor.serializedObject.ApplyModifiedProperties();
    }
  }

  void OnRemoveTransition(ReorderableList list) {
    var property = list.serializedProperty;
    var element = GetSelectedProperty(list);
    var transition = element.objectReferenceValue as StateTransitionAsset;
    _transitionEditors.Remove(element.objectReferenceValue);
    if (transition != null) {
      transition.Destroy();
    }
    // Needs to delete twice to remove it from the list
    property.DeleteArrayElementAtIndex(list.index);
    property.DeleteArrayElementAtIndex(list.index);
  }

  SerializedProperty GetSelectedProperty(ReorderableList list) {
    return list.serializedProperty.GetArrayElementAtIndex(list.index);
  }

  Editor GetEditor(Object obj) {
    Editor editor;
    if (!_transitionEditors.TryGetValue(obj, out editor)) {
      editor = Editor.CreateEditor(obj);
      _transitionEditors[obj] = editor;
    }
    return editor;
  }

}

}
