using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace Crescendo.API.Editor {

    public class CharacterEditorWindow : SelectableEditorWindow<GameObject> {

        public CharacterEditorData Target {
            get { return _target; }
            set {
                if (_target == value)
                    return;
                _target = value;
                OnTargetChange();
            }
        }

        private enum Tab {
            General = 0,
            Palette
        }

        private Tab tab;
        private string[] tabNames;

        private CharacterEditorData _target;
        private SerializedObject serializedObject;
        private List<string> ignoredProperties;

        private Dictionary<string, string> characters;
            
        [MenuItem("Window/Character")]
        public static CharacterEditorWindow ShowWindow() {
            return GetWindow<CharacterEditorWindow>("Character");
        }

        void OnEnable() {

            // Filter only GameObjects with Character scripts on either itself or one of it's ancestors
            SelectionMode = SelectionMode.TopLevel | SelectionMode.ExcludePrefab;
            Filter = o => o.GetComponentInParent<Character>() != null;

            tabNames = Enum.GetNames(typeof(Tab));
            ignoredProperties = new List<string>();
        }

        void OnTargetChange() {
            if (Target == null) {
                serializedObject = null;
                return;
            }

            serializedObject = new SerializedObject(Target);
        }

        protected override void OnSelectionChange() {
            base.OnSelectionChange();
            foreach(var selected in Selection)
                Debug.Log(selected);
        }

        void OnGUI() {
            Toolbar();
            EditArea();
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
        
        #region General Data Editor
        private void GeneralData() {
            if (Target == null)
                return;
            if(serializedObject == null)
                serializedObject = new SerializedObject(Target);

            ignoredProperties.Clear();
            ignoredProperties.Add("m_Script");

            DisplayBasicData();

            SerializedProperty current = serializedObject.GetIterator();
            current.Next(true);
            while (current.NextVisible(false)) {
                if (ignoredProperties.Contains(current.name))
                    continue;
                EditorGUILayout.PropertyField(current);
            }

            if (Target.Prefab != null) {
                ShowMaterialData();
            }
                
            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        void DisplayBasicData() {
            EditorGUILayout.LabelField("Full Name", Target.FullName);
            EditorGUILayout.LabelField("Internal Name", Target.InternalName);
        }

        void ShowMaterialData() {
            HashSet<Material> materials = new HashSet<Material>();
            foreach (var renderer in EditorUtil.GetComponentsInChildren<Renderer>(Target.Prefab)) {
                //Debug.Log(renderer);
                //materials.UnionWith(renderer.materials);
          }

            foreach(var material in materials)
                EditorGUILayout.LabelField(material.name);

        }

        #endregion

        void PropertyField(string name, SerializedProperty parent = null, Action<SerializedProperty> drawCall = null) {
            SerializedProperty property = parent == null
                                              ? serializedObject.FindProperty(name)
                                              : parent.FindPropertyRelative(name);
            if (drawCall != null)
                drawCall(property);
            else
                EditorGUILayout.PropertyField(property);
            ignoredProperties.Add(property.name);
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
                CreateCharacterDialog.Show();
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
                CreateCharacterDialog.Show();

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