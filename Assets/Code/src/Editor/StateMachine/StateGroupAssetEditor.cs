using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[CustomEditor(typeof(StateGroupAsset))]
public class StateGroupAssetEditor : BaseStateAssetEditor {

  ReorderableList _memberStateList;

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  protected override void OnEnable() {
    base.OnEnable();
    _memberStateList = new ReorderableList(
      serializedObject,
      serializedObject.FindProperty("States"),
      draggable: true, 
      displayHeader: true, 
      displayAddButton: true, 
      displayRemoveButton: true
    );
    _memberStateList.drawElementCallback += OnMemberGUI;
    _memberStateList.onAddCallback += OnAddMember;
  }

  protected override void OnStateGUI() => _memberStateList.DoLayoutList();

  void OnMemberGUI(Rect rect, int index, bool isActive, bool isFocused) {
    var obj= _memberStateList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;
    EditorGUI.LabelField(rect, obj.name);
  }

  void OnAddMember(ReorderableList list) {
    var stateMachine = serializedObject.FindProperty("_stateMachine").objectReferenceValue as StateMachineAsset;
    var genericMenu = new GenericMenu();
    var states = new List<StateAsset>(stateMachine.States.OfType<StateAsset>());
    var property = list.serializedProperty;
    for (var i = 0; i < property.arraySize; i++) {
      states.Remove(property.GetArrayElementAtIndex(i).objectReferenceValue as StateAsset);
    }
    foreach (var state in states) {
      genericMenu.AddItem(new GUIContent(state.name), false, () => {
        var index = Mathf.Max(list.index, 0);
        property.InsertArrayElementAtIndex(index);
        property.GetArrayElementAtIndex(index).objectReferenceValue = state;
        property.serializedObject.ApplyModifiedProperties();
      });
    }
    genericMenu.ShowAsContext();
  }

}

}