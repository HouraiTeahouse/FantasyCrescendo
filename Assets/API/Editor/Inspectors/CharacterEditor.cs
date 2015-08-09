using System.Linq;
using UnityEngine;
using UnityEditor;
using Vexe.Editor.Drawers;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;

namespace  Crescendo.API.Editor {
    
    public class CharacterEditor : ObjectDrawer<Character> {

        private Character _character;
        private EditorMember _internalName;
        private CharacterEditorData _editorData;

        protected override void Initialize() {
            base.Initialize();
            _internalName = FindRelativeMember("_internalName");
            Debug.Log(_internalName.Value);
            _editorData =
                Resources.FindObjectsOfTypeAll<CharacterEditorData>()
                         .FirstOrDefault(data => data.InternalName.GenericEquals(_internalName.Value));
            Debug.Log(_editorData);
        }

        public override void OnGUI() {
            _editorData = gui.Object("Editor Data", _editorData);
            if (_editorData == null) {
                _internalName.Value = "";
                if (GUILayout.Button("Create Character Data")) {
                    CreateCharacterDialog.Show(_character);
                }
            } else {
                _internalName.Value = _editorData.InternalName;
            }
            MemberField();
        }

    }

}
