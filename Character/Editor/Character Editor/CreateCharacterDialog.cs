using Hourai.Editor;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Hourai.SmashBrew.Editor {

    public class CreateCharacterDialog : ScriptableWizard {

        private GUIContent _nameContent;
        private Character _sourceObject;
        private CharacterEditorData data;

        private void OnEnable() {
            data = CreateInstance<CharacterEditorData>();
        }

        public static void Show(Character reference = null) {
            var wizard = DisplayWizard<CreateCharacterDialog>("Create Character", "Create");
            wizard._sourceObject = reference;
            if (reference != null) {
                string[] nameParts = reference.name.Split(' ', '_', '/');
                if (nameParts.Length >= 1)
                    wizard.data.FirstName = nameParts[0];
                if (nameParts.Length >= 2)
                    wizard.data.LastName = nameParts[1];
            }
        }

        protected override bool DrawWizardGUI() {
            string oldName = data.InternalName;
            data.FirstName = EditorGUILayout.TextField("First Name", data.FirstName);
            data.LastName = EditorGUILayout.TextField("Last Name", data.LastName);
            data.NameStyle = (NameType) EditorGUILayout.EnumPopup("Name Style", data.NameStyle);

            data.InternalName = data.FullName.ToLower().Replace(' ', '_');

            EditorGUILayout.LabelField("Full Name", data.FullName);
            EditorGUILayout.LabelField("Internal Name", data.InternalName);
            return oldName == data.InternalName;
        }

        private void OnWizardCreate() {
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

            GameObject prefabSource;

            if (_sourceObject == null)
                prefabSource = new GameObject(data.InternalName);
            else
                prefabSource = _sourceObject.gameObject;

            prefabSource.GetOrAddComponent<Character>().InternalName = data.InternalName;

            Object prefab = prefabSource.GetPrefab();
            if (prefab == null)
                prefab = PrefabUtil.CreatePrefab(data.RootFolder, prefabSource);
            else
                AssetUtil.MoveAsset(data.RootFolder, prefab);

            data.Prefab = prefab as GameObject;

            Selection.activeGameObject = prefabSource;

            CharacterEditorWindow.ShowWindow().Target = data;
        }

    }

}