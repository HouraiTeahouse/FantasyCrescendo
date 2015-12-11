using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hourai.Editor {

    /// <summary>
    /// Removes the extra "Script" field on any editor derived from this.
    /// </summary>
    public abstract class ScriptlessEditor : UnityEditor.Editor {

        private List<string> _toIgnore = new List<string>();

        /// <summary>
        /// Unity Callback. Called when the Editor is first created.
        /// </summary>
        protected virtual void OnEnable() {
            _toIgnore = new List<string> {"m_Script"};
        }

        public void AddException(string propertyName) {
            _toIgnore.Add(propertyName);
        }

        /// <summary>
        /// A replacement to the old DrawDefaultInspector that does not include the Script field.
        /// </summary>
        public new void DrawDefaultInspector() {
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.Next(true);
            while (iterator.NextVisible(false)) {
                if (!_toIgnore.Contains(iterator.name))
                    EditorGUILayout.PropertyField(iterator, true);
            }

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Unity Callback. Called to draw the Editor's UI every GUI update.
        /// </summary>
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
        }

    }

    /// <summary>
    /// Creates a global fallback editor for all MonoBehaviour derived types that removes the extra "Script" field.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof (MonoBehaviour), true, isFallback = true)]
    internal sealed class MonoBehaviourEditor : ScriptlessEditor {

    }
    
    /// <summary>
    /// Creates a global fallback editor for all ScriptableObject derived types that removes the extra "Script" field.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof (ScriptableObject), true, isFallback = true)]
    internal sealed class ScriptableObjectEditor : ScriptlessEditor {

    }
}