using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

/// <summary>
/// Custom Eidtor for CharacterControllerBuilders.
/// </summary>
[CustomEditor(typeof(CharacterControllerBuilder), isFallback = true)]
public class CharacterControllerBuilderEditor : UnityEditor.Editor {

  string filter = string.Empty;
  bool defaultExpanded;

  void OnEnable() {
    var builder = target as CharacterControllerBuilder;
    builder.BuildCharacterControllerImpl(new StateControllerBuilder<CharacterState, CharacterContext>());
    EditorUtility.SetDirty(builder);
  }

  AnimationTrack GetOrCreateAnimationTrack(string name, TimelineAsset timeline, Action<AnimationTrack> prepareFunc = null) {
    var track = timeline.GetRootTracks().OfType<AnimationTrack>()
                                        .FirstOrDefault(t => t != null && t.name == name);
    if (track != null) {
        if (prepareFunc != null)
            prepareFunc(track);
    } else {
        track = timeline.CreateTrack<AnimationTrack>(null, name);
    }
    return track;
  }

  public override void OnInspectorGUI() {
    var prefabProperty = serializedObject.FindProperty("_prefab");
    EditorGUILayout.PropertyField(prefabProperty);
    filter = EditorGUILayout.TextField("Filter", filter);
    var text = prefabProperty.objectReferenceValue != null ? "Update Animator Controller" : "Create Animation Controller";
    EditorGUILayout.Space();
    HandleDefaults();
    EditorGUILayout.Space();
    var data = serializedObject.FindProperty("_data");
    for (var i = 0; i < data.arraySize; i++) {
      var element = data.GetArrayElementAtIndex(i);
      var name = element.FindPropertyRelative("Name").stringValue;
      if (!string.IsNullOrEmpty(filter) && !name.ToUpper().Contains(filter.ToUpper())) {
        continue;
      }
      var elementData = element.FindPropertyRelative("Data");
      var timeline = elementData.FindPropertyRelative("Timeline");
      var length = elementData.FindPropertyRelative("Length");
      var timelineAsset = timeline.objectReferenceValue as TimelineAsset;
      EditorGUILayout.PropertyField(elementData, new GUIContent(name), true);
      var newTimeline = timeline.objectReferenceValue as TimelineAsset;
      if (timelineAsset != newTimeline && newTimeline != null) {
        length.floatValue = (float)newTimeline.duration;
      }
    }
    if (GUI.changed) {
      serializedObject.ApplyModifiedProperties();
    }
  }

  void HandleDefaults() {
    var defaultValues = serializedObject.FindProperty("_default");
    if (defaultExpanded = EditorGUILayout.Foldout(defaultExpanded, "Default Values")) {
      EditorGUI.indentLevel++;
      EditDefault<float>(defaultValues, "RotationOffset", p => p.floatValue, (p, f) => p.floatValue = f);
      EditorGUI.indentLevel--;
    }
  }

  void EditDefault<T>(SerializedProperty defaultValue, string field, 
                       Func<SerializedProperty, T> getFunc,
                       Action<SerializedProperty, T> setFunc) {
    var relativeField = defaultValue.FindPropertyRelative(field);
    var oldValue = getFunc(relativeField);
    EditorGUILayout.PropertyField(relativeField);
    var newValue = getFunc(relativeField);
    if (!oldValue.Equals(newValue)) {
      var data = serializedObject.FindProperty("_data");
      for (var i = 0; i < data.arraySize; i++) {
        var element = data.GetArrayElementAtIndex(i);
        var elementData = element.FindPropertyRelative("Data");
        relativeField = elementData.FindPropertyRelative(field);
        if (!getFunc(relativeField).Equals(oldValue)) continue;
        setFunc(relativeField, newValue);
      }
    }
  }

}

}
