using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace Crescendo.API.Editor {

    public class CharacterEditorWindow : EditorWindow {

        public CharacterEditorData Target { get; set; }

        private enum Tab {
            General = 0,
            Palette
        }

        private Tab tab;
        private string[] tabNames;

        private Dictionary<string, string> characters;
            
        [MenuItem("Window/Character")]
        public static CharacterEditorWindow ShowWindow() {
            return GetWindow<CharacterEditorWindow>("Character");
        }

        void OnEnable() {
            tabNames = Enum.GetNames(typeof(Tab));
        }

        void OnGUI() {
            FindCharactersa();
            Toolbar();
            EditArea();
        }

        void FindCharactersa() {
            if(characters == null)
                characters = new Dictionary<string, string>();

            foreach (var path in AssetUtil.FindAssetPaths<CharacterEditorData>()) {
                string characterName = Regex.Replace(path, ".*/(.*?)\\.asset", "$1");
                characters[characterName] = path;
            }
        }

        private void EditArea() {
            if (Target == null)
                SelectOrCreate();
            switch (tab) {
                case Tab.General:
                    GeneralData();
                    break;
                case Tab.Palette:
                    PalleteEidtor();
                    break;
            }
        }

        private void GeneralData() {
            if (Target == null)
                return;

            EditorGUILayout.LabelField(Target.InternalName);
        }

        private void PalleteEidtor() {
            
        }

        private void SelectOrCreate() {
            EditorGUILayout.BeginHorizontal();
            EditorUtil.Space();
            EditorGUILayout.LabelField("Select or create a Character:");
            EditorUtil.Space();
            EditorGUILayout.EndHorizontal();

            if(GUILayout.Button("Create a new Character"))
                CreateCharacterPrompt();

            foreach (var character in characters) {
                if (GUILayout.Button(character.Key)) {
                    Target = AssetDatabase.LoadAssetAtPath<CharacterEditorData>(character.Value);
                }
            }
        }

        private void CreateCharacterPrompt() {
            ScriptableWizard.DisplayWizard<CreateCharacterDialog>("Create Character", "Create");
        }

        #region Toolbar
        void Toolbar() {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            ToolbarLeft();
            EditorUtil.Space();
            ToolbarRight();
            EditorGUILayout.EndHorizontal();
        }

        void ToolbarLeft() {
            if (GUILayout.Button("Create", EditorStyles.toolbarButton))
                CreateCharacterPrompt();

            if (Target == null)
                return;

            if (GUILayout.Button("Unload", EditorStyles.toolbarButton))
                Target = null;

            if (GUILayout.Button("Build", EditorStyles.toolbarButton))
                Target.Generate();
        }

        void ToolbarRight() {
            if (Target == null)
                return;

            tab = (Tab)GUILayout.SelectionGrid((int)tab, tabNames, tabNames.Length, EditorStyles.toolbarButton);
        }
        #endregion
    }
}