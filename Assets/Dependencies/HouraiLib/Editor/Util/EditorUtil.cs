using System;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    public static class EditorUtil {

        internal class HorizontalArea : IDisposable {

            public HorizontalArea(GUIStyle style, GUILayoutOption[] options) {
                if (options == null) {
                    if (style == null)
                        EditorGUILayout.BeginHorizontal();
                    else
                        EditorGUILayout.BeginHorizontal(style);
                } else {
                    if (style == null)
                        EditorGUILayout.BeginHorizontal(options);
                    else
                        EditorGUILayout.BeginHorizontal(style, options);
                }
            }

            public void Dispose() {
                EditorGUILayout.EndHorizontal();
            }
        }

        internal class VerticalArea : IDisposable {

            public VerticalArea(GUIStyle style, GUILayoutOption[] options) {
                if (options == null) {
                    if (style == null)
                        EditorGUILayout.BeginVertical();
                    else
                        EditorGUILayout.BeginVertical(style);
                } else {
                    if (style == null)
                        EditorGUILayout.BeginVertical(options);
                    else
                        EditorGUILayout.BeginVertical(style, options);
                }
            }

            public void Dispose() {
                EditorGUILayout.EndVertical();
            }
        }

        internal class PropertyArea : IDisposable {
            
            public PropertyArea(Rect position, GUIContent content, SerializedProperty property) {
                EditorGUI.BeginProperty(position, content, property);
            }

            public void Dispose() {
                EditorGUI.EndProperty();
            }
        }

        public static IDisposable Horizontal(GUIStyle style = null, params GUILayoutOption[] options) {
            return new HorizontalArea(style, options);
        }

        public static IDisposable Vertical(GUIStyle style = null, params GUILayoutOption[] options) {
            return new VerticalArea(style, options);
        }

        public static IDisposable Property(GUIContent content, Rect position, SerializedProperty property) {
            return new PropertyArea(position, content, property);
        }

        public static string Text(string label, string text) {
            return EditorGUILayout.TextField(text, label);
        }

        public static void Space(int? space = null) {
            if (space == null)
                GUILayout.FlexibleSpace();
            else
                GUILayout.Space((int) space);
        }
    }
}
