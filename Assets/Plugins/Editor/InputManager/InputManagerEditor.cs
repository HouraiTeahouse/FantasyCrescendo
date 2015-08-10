using System;
using TeamUtility.Editor.IO.InputManager;
using TeamUtility.IO;
using UnityEditor;
using UnityEngine;

namespace TeamUtility.Editor {

    [CustomEditor(typeof (InputManager))]
    public sealed class InputManagerEditor : UnityEditor.Editor {

        private string[] _axisOptions = {"X", "Y", "3rd(Scrollwheel)", "4th", "5th", "6th", "7th", "8th", "9th", "10th"};

        private GUIContent _deadZoneInfo = new GUIContent("Dead Zone",
                                                          "Size of analog dead zone. Values within this range map to neutral.");

        private SerializedProperty _defaultConfiguration;
        private SerializedProperty _dontDestroyOnLoad;
        private bool _editingAltNegativeKey;
        private bool _editingAltPositiveKey;
        private bool _editingNegativeKey;
        private bool _editingPositiveKey;

        private GUIContent _gravityInfo = new GUIContent("Gravity",
                                                         "The speed(in units/sec) at which a digital axis falls towards neutral.");

        private SerializedProperty _ignoreTimescale;
        private SerializedProperty _intputConfigurations;
        private string[] _joystickOptions = {"Joystick 1", "Joystick 2", "Joystick 3", "Joystick 4"};
        private string _keyString;

        private GUIContent _sensitivityInfo = new GUIContent("Sensitivity",
                                                             "The speed(in units/sec) at which an axis moves towards the target value.");

        private GUIContent _snapInfo = new GUIContent("Snap",
                                                      "If input switches direction, do we snap to neutral and continue from there? For digital axes only.");

        private void OnEnable() {
            EditorToolbox.ShowStartupWarning();
            _intputConfigurations = serializedObject.FindProperty("inputConfigurations");
            _dontDestroyOnLoad = serializedObject.FindProperty("dontDestroyOnLoad");
            _ignoreTimescale = serializedObject.FindProperty("ignoreTimescale");
            _defaultConfiguration = serializedObject.FindProperty("defaultConfiguration");
        }

        public override void OnInspectorGUI() {
            var inputManager = target as InputManager;

            serializedObject.Update();
            GUILayout.Space(5.0f);

            GUILayout.BeginHorizontal();
            GUI.enabled = !AdvancedInputEditor.IsOpen;
            if (GUILayout.Button("Advanced\nEditor", GUILayout.Height(40.0f)))
                AdvancedInputEditor.OpenWindow(inputManager);
            GUI.enabled = true;
            if (GUILayout.Button("Create\nSnapshot", GUILayout.Height(40.0f)))
                EditorToolbox.CreateSnapshot(inputManager);
            GUI.enabled = EditorToolbox.CanLoadSnapshot();
            if (GUILayout.Button("Load\nSnapshot", GUILayout.Height(40.0f)))
                EditorToolbox.LoadSnapshot(inputManager);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(5.0f);
            EditorGUILayout.PropertyField(_defaultConfiguration);
            EditorGUILayout.PropertyField(_dontDestroyOnLoad);
            EditorGUILayout.PropertyField(_ignoreTimescale);
            EditorGUILayout.PropertyField(_intputConfigurations);
            if (_intputConfigurations.isExpanded) {
                EditorGUI.indentLevel++;
                int arraySize = EditorGUILayout.IntField("Size", _intputConfigurations.arraySize);
                if (arraySize != _intputConfigurations.arraySize)
                    _intputConfigurations.arraySize = arraySize;

                for (var i = 0; i < _intputConfigurations.arraySize; i++)
                    DisplayInputConfigurations(_intputConfigurations.GetArrayElementAtIndex(i));

                EditorGUI.indentLevel--;
            }

            GUILayout.Space(5.0f);
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayInputConfigurations(SerializedProperty inputConfig) {
            EditorGUILayout.PropertyField(inputConfig);
            if (!inputConfig.isExpanded)
                return;

            SerializedProperty name = inputConfig.FindPropertyRelative("name");
            SerializedProperty axes = inputConfig.FindPropertyRelative("axes");

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(axes);
            if (axes.isExpanded) {
                EditorGUI.indentLevel++;
                int arraySize = EditorGUILayout.IntField("Size", axes.arraySize);
                if (arraySize != axes.arraySize)
                    axes.arraySize = arraySize;

                for (var i = 0; i < axes.arraySize; i++)
                    DisplayAxisConfiguration(axes.GetArrayElementAtIndex(i));
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        private void DisplayAxisConfiguration(SerializedProperty axisConfig) {
            EditorGUILayout.PropertyField(axisConfig);
            if (!axisConfig.isExpanded)
                return;

            SerializedProperty name = axisConfig.FindPropertyRelative("name");
            SerializedProperty description = axisConfig.FindPropertyRelative("description");
            SerializedProperty positive = axisConfig.FindPropertyRelative("positive");
            SerializedProperty altPositive = axisConfig.FindPropertyRelative("altPositive");
            SerializedProperty negative = axisConfig.FindPropertyRelative("negative");
            SerializedProperty altNegative = axisConfig.FindPropertyRelative("altNegative");
            SerializedProperty deadZone = axisConfig.FindPropertyRelative("deadZone");
            SerializedProperty gravity = axisConfig.FindPropertyRelative("gravity");
            SerializedProperty sensitivity = axisConfig.FindPropertyRelative("sensitivity");
            SerializedProperty snap = axisConfig.FindPropertyRelative("snap");
            SerializedProperty invert = axisConfig.FindPropertyRelative("invert");
            SerializedProperty type = axisConfig.FindPropertyRelative("type");
            SerializedProperty axis = axisConfig.FindPropertyRelative("axis");
            SerializedProperty joystick = axisConfig.FindPropertyRelative("joystick");

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(description);

            //	Positive Key
            EditorToolbox.KeyCodeField(ref _keyString,
                                       ref _editingPositiveKey,
                                       "Positive",
                                       "inspector_positive_key",
                                       PropertyToKeyCode(positive));
            ProcessKeyString(ref _editingPositiveKey, positive);

            //	Negative Key
            EditorToolbox.KeyCodeField(ref _keyString,
                                       ref _editingNegativeKey,
                                       "Negative",
                                       "inspector_negative_key",
                                       PropertyToKeyCode(negative));
            ProcessKeyString(ref _editingNegativeKey, negative);

            //	Alt Positive Key
            EditorToolbox.KeyCodeField(ref _keyString,
                                       ref _editingAltPositiveKey,
                                       "Alt Positive",
                                       "inspector_alt_positive_key",
                                       PropertyToKeyCode(altPositive));
            ProcessKeyString(ref _editingAltPositiveKey, altPositive);

            //	Alt Negative Key
            EditorToolbox.KeyCodeField(ref _keyString,
                                       ref _editingAltNegativeKey,
                                       "Alt Negative",
                                       "inspector_alt_negative_key",
                                       PropertyToKeyCode(altNegative));
            ProcessKeyString(ref _editingAltNegativeKey, altNegative);

            EditorGUILayout.PropertyField(gravity, _gravityInfo);
            EditorGUILayout.PropertyField(deadZone, _deadZoneInfo);
            EditorGUILayout.PropertyField(sensitivity, _sensitivityInfo);
            EditorGUILayout.PropertyField(snap, _snapInfo);
            EditorGUILayout.PropertyField(invert);
            EditorGUILayout.PropertyField(type);
            axis.intValue = EditorGUILayout.Popup("Axis", axis.intValue, _axisOptions);
            joystick.intValue = EditorGUILayout.Popup("Joystick", joystick.intValue, _joystickOptions);
            EditorGUI.indentLevel--;
        }

        private KeyCode PropertyToKeyCode(SerializedProperty key) {
            return AxisConfiguration.StringToKey(key.enumNames[key.enumValueIndex]);
        }

        private void ProcessKeyString(ref bool isEditing, SerializedProperty key) {
            if (isEditing && Event.current.type == EventType.KeyUp) {
                KeyCode keyCode = AxisConfiguration.StringToKey(_keyString);
                if (keyCode == KeyCode.None) {
                    key.enumValueIndex = 0;
                    _keyString = string.Empty;
                } else {
                    key.enumValueIndex = IndexOfKeyName(key.enumNames, _keyString);
                    _keyString = keyCode.ToString();
                }
            }
        }

        private int IndexOfKeyName(string[] array, string name) {
            for (var i = 0; i < array.Length; i++) {
                if (string.Compare(name, array[i], StringComparison.InvariantCultureIgnoreCase) == 0)
                    return i;
            }

            return 0;
        }

    }

}