using UnityEngine;
using System.Collections;
using Crescendo.API.Editor;
using UnityEditor;

namespace Crescendo.API {

    public class CreateCharacterDialog : ScriptableWizard {

        private CharacterEditorData data;

        private GUIContent _nameContent;

        void OnEnable() {
            data = CreateInstance<CharacterEditorData>();
        }

        protected override bool DrawWizardGUI() {
            string oldName = data.InternalName;
            data.FirstName = EditorGUILayout.TextField("First FullName", data.FirstName);
            data.LastName = EditorGUILayout.TextField("Last FullName", data.LastName);
            data.NameStyle = (NameType) EditorGUILayout.EnumPopup("FullName Style", data.NameStyle);

            data.InternalName = data.FullName.ToLower().Replace(' ', '_');

            EditorGUILayout.LabelField("Full FullName", data.FullName);
            EditorGUILayout.LabelField("Internal FullName", data.InternalName);
            return oldName == data.InternalName;
        }

        void OnWizardCreate() {
            // Check if the character data already exists
            if (AssetDatabase.IsValidFolder(data.RootFolder)) {
                bool result = EditorUtility.DisplayDialog("Error",
                                                            "A character with the internal name of \"" + data.InternalName +
                                                            "\" already exists. Either edit the existing Character, or delete and restart.",
                                                            "Delete",
                                                            "Cancel");
                if (result)
                    AssetDatabase.DeleteAsset(data.RootFolder);
                else
                    return;
            }

            data.Generate();

            CharacterEditorWindow.ShowWindow().Target = data;
        }
    }
}
