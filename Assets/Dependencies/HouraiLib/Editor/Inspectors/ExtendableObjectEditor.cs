using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse.Editor {

    [InitializeOnLoad]
    [CustomEditor(typeof(ExtendableObject), isFallback = true)]
    public class ExtendableObjectEditor : BaseEditor<ExtendableObject> {

        public class ExtensionType {
            public Type Type;
            public ExtensionAttribute Attribute;
        }

        static readonly Dictionary<Type, ExtensionType[]> _matches;
        ObjectSelector<ExtensionType> Selector;

        static ExtendableObjectEditor() {
            _matches = ReflectionUtilty.AllTypes
                            .ConcreteClasses()
                            .IsAssignableFrom(typeof(ScriptableObject))
                            .WithAttribute<ExtensionAttribute>()
                            .Select(k => new ExtensionType {Attribute = k.Value, Type = k.Key})
                            .GroupBy(et => et.Attribute.TargetType)
                            .ToDictionary(g => g.Key, g => g.ToArray());
        }

        IEnumerable<ExtensionType> GetTypes(bool required) {
            var type = target.GetType();
            foreach (Type interfaceType in type.GetInterfaces()) {
                if (!_matches.ContainsKey(interfaceType))
                    continue;
                foreach (ExtensionType extensionType in _matches[interfaceType])
                    if(required == extensionType.Attribute.Required)
                        yield return extensionType;
            }
            while(type != null) {
                if (_matches.ContainsKey(type)) {
                    foreach (ExtensionType extensionType in _matches[type])
                        if(required == extensionType.Attribute.Required)
                            yield return extensionType;
                }
                type = type.BaseType;
            }
        }

        void OnEnable() {
            Selector = new ObjectSelector<ExtensionType>(t => t.Type.Name) {
                Selections = GetTypes(false).ToArray()
            };
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            DrawExtensionGUI();
        }

        public void DrawExtensionGUI() {
            if (Target == null)
                return;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Extensions", EditorStyles.boldLabel);
            using(hGUI.Horizontal()) {
                var selection = Selector.Draw(GUIContent.none);
                if (GUILayout.Button("Add") && selection != null) {
                    Undo.RecordObject(Target, "Add Extension");
                    Target.AddExtension(selection.Type);
                    Repaint();
                }
            }
            foreach (ScriptableObject extension in Target.Extensions.ToArray()) {
                using(hGUI.Horizontal()) {
                    EditorGUILayout.InspectorTitlebar(true, extension);
                    if(GUILayout.Button(GUIContent.none, "ToggleMixed", GUILayout.Width(15))) {
                        Undo.RecordObject(Target, "Remove Extension");
                        Target.RemoveExtension(extension);
                        Repaint();
                    }
                }
                EditorGUILayout.Space();
                if(extension != null)
                    CreateEditor(extension).OnInspectorGUI();
            }
        }

    }
}
