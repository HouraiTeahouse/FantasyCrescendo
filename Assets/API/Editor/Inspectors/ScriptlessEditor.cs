using UnityEngine;
using UnityEditor;

namespace Crescendo.API.Editor{
    
    /// <summary>
    /// Removes the extra "Script" field on any editor derived from this.
    /// </summary>
    public abstract class ScriptlessEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.Next(true);
            while (iterator.NextVisible(false)) {
                if (iterator.name != "m_Script")
                    EditorGUILayout.PropertyField(iterator, true);
            }
        }

    }

    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    internal sealed class MonoBehaviourEditor : ScriptlessEditor {
    }

    [CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
    internal sealed class ScriptableObjectEditor : ScriptlessEditor {
    }

}