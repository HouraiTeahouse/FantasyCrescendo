using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace Crescendo.API.Editor {
    
    public static class EditorUtil {

/*       
        This somehow generates a compiler error. Commenting it out for now.

        public class HorizontalArea : IDisposable {

            public HorizontalArea(GUIStyle style, GUILayoutOption[] options) {
                if (options == null) {
                    if (style == null) {
                        EditorGUILayout.BeginHorizontal();
                    } else {
                        EditorGUILayout.BeginHorizontal(style);
                    }
                } else {
                    if (style == null) {
                        EditorGUILayout.BeginHorizontal(options);
                    } else {
                        EditorGUILayout.BeginHorizontal(style, options);
                    }
                }
            }
            
            public void Dispose() {
                EditorGUILayout.EndHorizontal();
            }
        }

        public static IDisposable Horizontal(GUIStyle style = null, params GUILayoutOption[] options) {
            return new HorizontalArea(style, options);
        }*/
        
        public static string Text(string label, string text) {
            return EditorGUILayout.TextField(text, label);
        }

        public static void Space(int? space = null) {
            if(space == null)
                GUILayout.FlexibleSpace();
            else
                GUILayout.Space((int)space);
        }

    }

}
