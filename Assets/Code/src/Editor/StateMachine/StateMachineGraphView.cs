using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateMachineGraphView : GraphView {
}

/// <summary>
/// Root Visual element which takes up entire editor window. Every other visual element is a child
/// of this.
/// </summary>
public class StateMachineGraphEditorView : VisualElement {

    TestEditorWindow _editorWindow;
    GraphView _graphView;

    public StateMachineGraphEditorView(TestEditorWindow editorWindow) {
        _editorWindow = editorWindow;

        // Add Toolbar
        Add(new IMGUIContainer(() => {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton)) {
                // SaveRequested?.Invoke();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }));

        var content = new VisualElement {name = "content"};
        {
            _graphView = new StateMachineGraphView {
                name = "State Machine Graph View",
            };

            _graphView.SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
            _graphView.AddManipulator(new ContentDragger());
            _graphView.AddManipulator(new SelectionDragger());
            _graphView.AddManipulator(new RectangleSelector());
            _graphView.AddManipulator(new ClickSelector());
            // _graphView.RegisterCallback<KeyDownEvent>(KeyDown);
            content.Add(_graphView);

            // _graphView.graphViewChanged = GraphViewChanged;
        }
        Add(content);
    }

}

}
