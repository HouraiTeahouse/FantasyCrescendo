using System;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    public static class hGUI {

        class PropertyDisposable : IDisposable {

            public PropertyDisposable(GUIContent label, Rect position, SerializedProperty property) {
                EditorGUI.BeginProperty(position, label, property);
            }

            public void Dispose() {
                EditorGUI.EndProperty();
            }
        
        }

        class ColorDisposable : IDisposable {

            readonly Color color;

            public ColorDisposable(Color newColor) {
                color = GUI.color;
                GUI.color = newColor;
            }

            public void Dispose() { GUI.color = color; }
        }

        class BackgroundColorDisposable : IDisposable {
            readonly Color color;

            public BackgroundColorDisposable(Color newColor) {
                color = GUI.backgroundColor;
                GUI.backgroundColor = newColor;
            }

            public void Dispose() { GUI.backgroundColor = color; }
        }

        class EnabledDisposable : IDisposable {
            readonly bool state;

            public EnabledDisposable(bool state) {
                this.state = GUI.enabled;
                GUI.enabled = state;
            }

            public void Dispose() { GUI.enabled = state; }
        }

        public static GUIContent BuiltinContent(string path) {
            return new GUIContent(EditorGUIUtility.FindTexture(path));
        }

        public static bool ToolbarButton(string content,
                                  params GUILayoutOption[] options) {
            return GUILayout.Button(content, EditorStyles.toolbarButton, options);
        }

        public static bool ToolbarButton(GUIContent content,
                                  params GUILayoutOption[] options) {
            return GUILayout.Button(content, EditorStyles.toolbarButton, options);
        }

        public static bool ToolbarToggle(GUIContent content,
                                         bool value,
                                         params GUILayoutOption[] options) {
            return GUILayout.Toggle(value, content, EditorStyles.toolbarButton, options);
        }

        public static bool ToolbarToggle(string content,
                                         bool value,
                                         params GUILayoutOption[] options) {
            return GUILayout.Toggle(value, content, EditorStyles.toolbarButton, options);
        }

        public static void Space(float size = -1) {
            if(size > 0)
                GUILayout.Space(size);
            else
                GUILayout.FlexibleSpace();
        }

        public static IDisposable Property(GUIContent label,
                                           Rect position,
                                           SerializedProperty property) {
            return new PropertyDisposable(label, position, property);
        }

        public static IDisposable Enabled(bool state) {
            return new EnabledDisposable(state);
        }

        public static IDisposable Color(Color color) {
            return new ColorDisposable(color);
        }

        public static IDisposable BackgroundColor(Color color) {
            return new BackgroundColorDisposable(color);
        }

        public static EditorGUILayout.HorizontalScope Horizontal(GUIStyle style,
                                             params GUILayoutOption[] options) {
            return new EditorGUILayout.HorizontalScope(style, options);
        }

        public static EditorGUILayout.HorizontalScope Horizontal(params GUILayoutOption[] options) {
            return new EditorGUILayout.HorizontalScope(options);
        }

        public static EditorGUILayout.VerticalScope Vertical(GUIStyle style,
                                             params GUILayoutOption[] options) {
            return new EditorGUILayout.VerticalScope(style, options);
        }

        public static EditorGUILayout.VerticalScope Vertical(params GUILayoutOption[] options) {
            return new EditorGUILayout.VerticalScope(options);
        }

    }
}
