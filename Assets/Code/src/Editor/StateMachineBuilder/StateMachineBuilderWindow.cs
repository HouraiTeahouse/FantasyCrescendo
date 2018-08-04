using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace HouraiTeahouse.FantasyCrescendo {
  public class StateMachineBuilderWindow : EditorWindow {
    private const float UpperTabHeight = 20;
    private readonly Vector2 NodeSize = new Vector2(120, 50);
    private readonly Vector2 NodeTextSize = new Vector2(60, 20);

    private bool initDone = false;
    private StateMachineAsset smAsset;
    private GUIStyle labelStyle;

    private Texture2D lineTexture;

    private StateMachineAsset.StateMachineNode sourceNode = null;

    [MenuItem("Window/State Machine Builder Window")]
    static void Init() {
      GetWindow(typeof(StateMachineBuilderWindow)).Show();
    }

    private void OnEnable() {
      initDone = false;
      sourceNode = null;
    }

    private void InitStyles() {
      initDone = true;

      labelStyle = GUI.skin.label;
      labelStyle.alignment = TextAnchor.MiddleCenter;
      labelStyle.richText = true;
      labelStyle.fontSize = 15;

      lineTexture = new Texture2D(1, 2);
      lineTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.5f));
      lineTexture.SetPixel(0, 1, Color.white);
      lineTexture.Apply();
    }

    private void OnGUI() {
      // Initialize
      if (smAsset == null)
        smAsset = StateMachineAsset.GetStateMachineAsset();

      if (!initDone)
        InitStyles();

      // Draw lines (underneath all)
      DrawLinkLines();

      // Draw top bar buttons
      GUILayout.BeginHorizontal(GUILayout.MaxHeight(UpperTabHeight));
      if (GUILayout.Button("Add Node")) {
        smAsset.AddNode();
      }
      if (GUILayout.Button("Build")) {
        //CCBuilder.UpdateFile();
      }
      GUILayout.EndHorizontal();

      // Draw each window
      BeginWindows();
      foreach (var item in smAsset.StateNodes) {
        item.Window = GUI.Window(item.Id, new Rect(item.Window.position, NodeSize), DrawNode, "", GUIStyle.none);
      }
      EndWindows();

      HandleLinkInput();
      DrawMouseLine();

      EditorUtility.SetDirty(smAsset);
    }

    private void OnInspectorUpdate() {
      if (sourceNode != null)
        Repaint();
    }

    private void DrawNode(int id) {
      var node = smAsset.NodeDictionary[id];
      var e = Event.current;

      // Reposition so it doesn't get too out of screen
      node.Window.position = new Vector2(Mathf.Max(node.Window.position.x, 0), Mathf.Max(node.Window.position.y, UpperTabHeight));
      
      // If touched, switch to that node's scriptable object
      if (e.button == 0 && e.type == EventType.MouseDown){
        Selection.activeObject = node.Asset;
      }

      // Draw the damn box and text
      GUILayout.BeginArea(new Rect(Vector2.zero, node.Window.size), new GUIStyle("Box"));
      var rect = new Rect((node.Window.size - NodeTextSize) / 2, NodeTextSize);
      GUI.Label(rect, new GUIContent(node.Asset.State), labelStyle);
      GUILayout.EndArea();

      // If right clicking, don't even try to drag
      if (e.button != 1) {
        GUI.DragWindow();
      }
    }

    private void TryToConnectTwoNodes(StateMachineAsset.StateMachineNode destinationNode) {
      if (destinationNode != null && sourceNode != destinationNode && !sourceNode.TransitionExists(destinationNode)) {
        sourceNode.AddTransition(destinationNode);
      }
      sourceNode = null;
      Repaint();
    }

    private void HandleLinkInput(){
      Event e = Event.current;
      if (e.button == 1) {
        switch (e.type) {
          case EventType.MouseDown:
            foreach (var item in smAsset.StateNodes) {
              if (item.Window.Contains(e.mousePosition)) {
                sourceNode = item;
                break;
              }
            }
            break;
          case EventType.MouseUp:
            if (sourceNode == null) break;
            StateMachineAsset.StateMachineNode temp = null;
            foreach (var item in smAsset.StateNodes) {
              if (item.Window.Contains(e.mousePosition)) {
                temp = item;
                break;
              }
            }
            TryToConnectTwoNodes(temp);
            break;
        }
      }
    }

    private void DrawMouseLine(){
      Handles.BeginGUI();
      Handles.color = Color.black;

      if (sourceNode != null) {
        var direction = (Event.current.mousePosition - sourceNode.GetCenter).normalized;
        Handles.DrawAAPolyLine(lineTexture, 3, sourceNode.GetDrawLineEnd(direction), Event.current.mousePosition);
      }

      Handles.EndGUI();
    }

    private void DrawLinkLines() {
      Handles.BeginGUI();
      Handles.color = Color.black;

      foreach (var node in smAsset.StateNodes) {
        foreach (var transition in node.TransitionIds.Select(t => smAsset.NodeDictionary[t])) {
          var direction = (transition.GetCenter - node.GetCenter).normalized;
          var startPoint = node.GetDrawLineEnd(direction);
          var endPoint = transition.GetDrawLineEnd(direction);
          var midPoint = (endPoint + startPoint) / 2;
          Handles.DrawAAPolyLine(lineTexture, 2, startPoint, endPoint);
          Handles.DrawAAPolyLine(lineTexture, 2, node.GetDrawLineArrowEnd(direction, midPoint, Vector3.forward), midPoint,
                                                  node.GetDrawLineArrowEnd(direction, midPoint, Vector3.back));
        }
      }

      Handles.EndGUI();
    }

    /// <summary>
    /// Add copy-paste functionality to any text field
    /// Returns changed text or NULL.
    /// Usage: text = HandleCopyPaste (controlID) ?? text;
    /// </summary>
    public static string HandleCopyPaste(int controlID) {
      if (controlID == GUIUtility.keyboardControl) {
        var e = Event.current;
        if (e.type == EventType.ValidateCommand) {
          if (e.commandName == "Copy" || e.commandName == "Paste" || e.commandName == "SelectAll" || e.commandName == "Cut") {
            e.Use();
          }
        } else if (e.type == EventType.ExecuteCommand) {
          TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
          if (e.commandName == "Copy") {
            editor.Copy();
          } else if (e.commandName == "Paste") {
            editor.Paste();
            return editor.text;
          } else if (e.commandName == "SelectAll") {
            editor.SelectAll();
          } else if (e.commandName == "Cut") {
            editor.Cut();
            return editor.text;
          }
        }
      }
      return null;
    }

    /// <summary>
    /// TextField with copy-paste support
    /// </summary>
    public static string TextField(Rect rect, string value, GUIStyle style) {
      int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
      if (textFieldID == 0)
        return value;

      // Handle custom copy-paste
      value = HandleCopyPaste(textFieldID) ?? value;

      GUI.SetNextControlName("NodeTextField");
      return GUI.TextField(rect, value, style);
    }
  }
}
