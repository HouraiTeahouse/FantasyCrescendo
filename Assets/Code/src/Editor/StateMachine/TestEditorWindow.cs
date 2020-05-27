using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class TestEditorWindow : LockableEditorWindow {

    [MenuItem("Window/Test Builder Window")]
    static void Init() => GetWindow(typeof(TestEditorWindow), false, "Test").Show();

    void OnEnable()  {}

    StateMachineGraphEditorView _graphEditorView;
    string _selectedGuid;

    StateMachineGraphEditorView LogicGraphEditorView {
        get => _graphEditorView;
        set {
            if (_graphEditorView != null) {
                _graphEditorView.RemoveFromHierarchy();
            }

            _graphEditorView = value;

            if (_graphEditorView != null) {
                rootVisualElement.Add(_graphEditorView);
            }
        }
    }

    public string SelectedGuid => _selectedGuid;

    public void Initialize(string guid) {
        try {
            _selectedGuid = guid;
            var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
            var path = AssetDatabase.GetAssetPath(asset);
            var textGraph = File.ReadAllText(path, Encoding.UTF8);

            LogicGraphEditorView = new StateMachineGraphEditorView(this);
            Repaint();
        } catch (Exception) {
            _graphEditorView = null;
            throw;
        }
    }

    void OnDisable() {
        LogicGraphEditorView = null;
    }

    void OnDestroy() {
        LogicGraphEditorView = null;
    }

    void Update() {
    }

}

}
