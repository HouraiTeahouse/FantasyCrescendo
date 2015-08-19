using System.Linq;
using UnityEngine;
using Vexe.Editor.Drawers;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;

namespace Hourai.SmashBrew.Editor {

    public class CharacterEditor : ObjectDrawer<Character> {

        private Character _character;
        private CharacterEditorData _editorData;
        private EditorMember _internalName;

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
                if (GUILayout.Button("Create Character Data"))
                    CreateCharacterDialog.Show(_character);
            } else
                _internalName.Value = _editorData.InternalName;
            MemberField();
        }

    }

}