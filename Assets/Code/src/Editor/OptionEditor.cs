using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse {

[CustomEditor(typeof(Option))]
public class OptionEditor : Editor {

  const string kDefaultValueField = "DefaultValue";
  const string kMinValueField = "MinValue";
  const string kMaxValueField = "MaxValue";
  const string kBadTypeMessage = "The provided type is not an enum type. May need to qualify with the assembly name.";
  const string kBadPathMessage = "The provided path cannot be used. Please provide a path.";

  public override void OnInspectorGUI() {
    var path = serializedObject.FindProperty("Path");
    var type = serializedObject.FindProperty("Type");
    EditorGUILayout.PropertyField(path);
    EditorGUILayout.PropertyField(type);
    Header("Values");
    switch ((OptionType)type.intValue) {
      case OptionType.Integer: IntGUI(); break;
      case OptionType.Float: FloatGUI(); break;
      case OptionType.Enum: EnumGUI(); break;
      case OptionType.Boolean: BoolGUI(); break;
    }
    UIGUI();
    MiscInfoGUI();
    EditorGUILayout.Space();
    if (string.IsNullOrEmpty(path.stringValue)) {
      EditorGUILayout.HelpBox(kBadPathMessage, MessageType.Error, true);
    }
    EnforceMinMax();
    if (GUI.changed) {
      serializedObject.ApplyModifiedProperties();
    }
  }

  void UIGUI() {
    Header("UI");
    EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplayName"));
    EditorGUILayout.PropertyField(serializedObject.FindProperty("Category"));
    EditorGUILayout.PropertyField(serializedObject.FindProperty("SortOrder"));
  }

  void MiscInfoGUI() {
    Header("Misc Info");
    EditorGUILayout.PropertyField(serializedObject.FindProperty("IsDebug"));
    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnValueChanged"));
  }

  void IntGUI() {
    IntField(kDefaultValueField);
    IntField(kMinValueField);
    IntField(kMaxValueField);
  }

  void FloatGUI() {
    EditorGUILayout.PropertyField(serializedObject.FindProperty(kDefaultValueField));
    EditorGUILayout.PropertyField(serializedObject.FindProperty(kMinValueField));
    EditorGUILayout.PropertyField(serializedObject.FindProperty(kMaxValueField));
  }

  void BoolGUI() {
    BoolField(kDefaultValueField);
    SetProperty(kMinValueField, 0f);
    SetProperty(kMaxValueField, 1f);
  }

  void EnumGUI() {
    var property = serializedObject.FindProperty("EnumType");
    EditorGUILayout.PropertyField(property);

    var enumType = Type.GetType(property.stringValue);
    if (enumType == null || !enumType.IsEnum) {
      EditorGUILayout.HelpBox(kBadTypeMessage, MessageType.Error, true);
      return;
    }
    EnumField(kDefaultValueField, enumType);
    var values = Enum.GetValues(enumType);
    float min = float.MaxValue;
    float max = float.MinValue;
    for (var i = 0; i < values.Length; i++) {
      int intValue = Convert.ToInt32(values.GetValue(i));
      min = Mathf.Min(min, intValue);
      max = Mathf.Max(max, intValue);
    }
    SetProperty(kMinValueField, min);
    SetProperty(kMaxValueField, max);
    EnumDisplayValues(enumType);
  }

  void EnumDisplayValues(Type enumType) {
    Header("Enum Display Names");
    var values = Enum.GetValues(enumType);
    var property = serializedObject.FindProperty("EnumOptions");
    var index = 0;
    for (; index < values.Length; index++) {
      string valueName = values.GetValue(index).ToString();
      if (index >= property.arraySize) {
        property.InsertArrayElementAtIndex(property.arraySize);
        property.GetArrayElementAtIndex(index).stringValue = valueName;
      }
      var item = property.GetArrayElementAtIndex(index);
      item.FindPropertyRelative("Value").intValue = Convert.ToInt32(values.GetValue(index));
      EditorGUILayout.PropertyField(item.FindPropertyRelative("DisplayName"), new GUIContent(valueName));
    }
    while (index < property.arraySize) {
      property.DeleteArrayElementAtIndex(property.arraySize - 1);
    }
  }

  void Header(string title)  {
    EditorGUILayout.Space();
    EditorGUILayout.LabelField(title, EditorStyles.boldLabel); 
  }

  void EnforceMinMax() {
    var defaultProperty = serializedObject.FindProperty(kDefaultValueField);
    var min = serializedObject.FindProperty(kMinValueField).floatValue;
    var max = serializedObject.FindProperty(kMaxValueField).floatValue;
    defaultProperty.floatValue = Mathf.Clamp(defaultProperty.floatValue, min, max);
  }

  void SetProperty(string property, float value) => serializedObject.FindProperty(property).floatValue = value;

  void EnumField(string property, Type enumType) {
    var serializedProperty = serializedObject.FindProperty(property);
    Enum value = (Enum)Enum.ToObject(enumType, (int)serializedProperty.floatValue);
    value = EditorGUILayout.EnumPopup(serializedProperty.displayName, value);
    serializedProperty.floatValue = (float)Convert.ToInt32(value);
  }

  void IntField(string property) {
    var serializedProperty = serializedObject.FindProperty(property);
    int value = (int)serializedProperty.floatValue;
    value = EditorGUILayout.IntField(serializedProperty.displayName, value);
    serializedProperty.floatValue = (float)value;
  }

  void BoolField(string property) {
    var serializedProperty = serializedObject.FindProperty(property);
    bool value = serializedProperty.floatValue != 0f;
    value = EditorGUILayout.Toggle(serializedProperty.displayName, value);
    serializedProperty.floatValue = value ? 1.0f : 0.0f;
  }

}
    
}
