using UnityEngine;
using System.Collections;
using UnityEditor;

namespace  Crescendo.API.Editor {
    
    [CustomEditor(typeof(Character))]
    public class CharacterEditor : ScriptlessEditor {

        private Character _character;
        private SerializedProperty _internalName;
        private CharacterEditorData _editorData;

        protected override void OnEnable() {
            _character = target as Character;
            _internalName = serializedObject.FindProperty("_internalName");
            AddException("_internalName");

            if(!string.IsNullOrEmpty(_internalName.stringValue))
                foreach (var path in AssetUtil.FindAssetPaths<CharacterEditorData>(_internalName.stringValue)) {
                    CharacterEditorData data = AssetDatabase.LoadAssetAtPath<CharacterEditorData>(path);
                    if (data == null && _internalName.stringValue == data.InternalName)
                        continue;
                    _editorData = data;
                    break;
                }
            _character.AttachRequiredComponents();
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            _editorData = (CharacterEditorData)EditorGUILayout.ObjectField("Editor Data",_editorData, typeof (CharacterEditorData));
            if (_editorData == null) {
                _internalName.stringValue = "";
                if (GUILayout.Button("Create Character Data")) {
                    CreateCharacterDialog.Show(_character);
                }
            } else {
                _internalName.stringValue = _editorData.InternalName;
            }

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        void OnDisable() {
            Debug.Log("Disabled");
        }

    }

}
