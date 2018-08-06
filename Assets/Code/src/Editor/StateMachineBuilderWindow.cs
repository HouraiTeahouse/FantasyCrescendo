using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

using Node = HouraiTeahouse.FantasyCrescendo.Characters.StateMachineMetadata.StateMachineNode;
using StateNode = HouraiTeahouse.FantasyCrescendo.Characters.StateMachineMetadata.StateMachineStateNode;
using TransitionNode = HouraiTeahouse.FantasyCrescendo.Characters.StateMachineMetadata.StateMachineTransitionNode;

namespace HouraiTeahouse.FantasyCrescendo.Characters {
  public class StateMachineBuilderWindow : EditorWindow {
    private const float UpperTabHeight = 20;
    private readonly Vector2 NodeSize = new Vector2(120, 50);
    private readonly Vector2 NodeTextSize = new Vector2(60, 20);

    private bool initDone = false;
    private StateMachineMetadata metaData;
    private GUIStyle labelStyle;

    private Texture2D lineTexture;

    private StateNode sourceNode = null;
    private Node selectedNode = null;

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
      if (metaData == null)
        metaData = StateMachineAsset.GetStateMachineAsset().Metadata;

      if (!initDone)
        InitStyles();

      // Update position vectors for transitions
      metaData.UpdateTransitionNodes();

      // Draw lines (underneath all)
      DrawLinkLines();

      // Draw UI (top bars and instructions)
      GUILayout.BeginVertical();
      GUILayout.BeginHorizontal(GUILayout.MaxHeight(UpperTabHeight));
      if (GUILayout.Button("Add State Node")) {
        var temp = metaData.AddStateNode();
        Selection.activeObject = temp.Asset;
        selectedNode = temp;
      }
      if (GUILayout.Button("Delete Selected Node") && selectedNode != null){
        if (selectedNode is StateNode) {
          metaData.RemoveStateNode(selectedNode as StateNode);
        } else if (selectedNode is TransitionNode) {
          metaData.RemoveTransitionNode(selectedNode as TransitionNode);
        }
        selectedNode = null;
      }
      if (GUILayout.Button("Build")) {
        //CCBuilder.UpdateFile();
      }
      GUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label(new GUIContent("Right click to create transitions"));

      GUILayout.EndHorizontal();
      GUILayout.EndVertical();

      // Draw each window
      BeginWindows();
      foreach (var item in metaData.StateNodes) {
        item.Window = GUI.Window(item.Id, new Rect(item.Window.position, NodeSize), DrawNode, "", GUIStyle.none);
      }
      EndWindows();

      // Gather input and draw line to mouse (if attempting a link)
      HandleMouseInput();
      DrawMouseLine();

      EditorUtility.SetDirty(metaData);
    }

    private void OnInspectorUpdate() {
      if (sourceNode != null)
        Repaint();
    }

    private void DrawNode(int id) {
      if (!metaData.StateDictionary.ContainsKey(id)) return;
      var node = metaData.StateDictionary[id];
      var e = Event.current;

      // Select once clicked
      if (e.button == 0 && e.type == EventType.MouseDown){
        Selection.activeObject = node.Asset;
        selectedNode = node;
      }

      // Reposition so it doesn't get too out of screen
      node.Window.position = new Vector2(Mathf.Max(node.Window.position.x, 0), Mathf.Max(node.Window.position.y, UpperTabHeight));

      // Draw the damn box and text
      GUILayout.BeginArea(new Rect(Vector2.zero, node.Window.size), new GUIStyle("Box"));
      var rect = new Rect((node.Window.size - NodeTextSize) / 2, NodeTextSize);
      GUI.Label(rect, 
        new GUIContent(string.Format(selectedNode == node ? "<b>{0}</b>": "{0}", node.Asset.name)), 
        labelStyle);
      GUILayout.EndArea();

      // If right clicking, don't even try to drag
      if (e.button != 1) {
        GUI.DragWindow();
      }
    }

    private void TryToConnectTwoNodes(StateNode destinationNode) {
      if (destinationNode != null 
        && sourceNode != destinationNode 
        && !metaData.TransitionNodeExists(sourceNode, destinationNode)) {
          var temp = metaData.AddTransitionNode(sourceNode, destinationNode);
          Selection.activeObject = temp.Asset;
          selectedNode = temp;
      }
      sourceNode = null;
      Repaint();
    }

    private void HandleMouseInput(){
      Event e = Event.current;
      switch (e.button){
        // Left Click
        case 0:
          if (e.type == EventType.MouseDown) {
            foreach (var item in metaData.TransitionNodes){
              if (item.Contains(e.mousePosition)){
                Selection.activeObject = item.Asset;
                selectedNode = item;
                Repaint();
                return;
              }
            }
          }

          break;
        // Right Click
        case 1:
          switch (e.type) {
            // Start link
            case EventType.MouseDown:
              foreach (var item in metaData.StateNodes) {
                if (item.Window.Contains(e.mousePosition)) {
                  sourceNode = item;
                  break;
                }
              }
              break;
            // End link
            case EventType.MouseUp:
              if (sourceNode == null) break;
              StateNode temp = null;
              foreach (var item in metaData.StateNodes) {
                if (item.Window.Contains(e.mousePosition)) {
                  temp = item;
                  break;
                }
              }
              TryToConnectTwoNodes(temp);
              break;
          }
          break;
      }

    }

    private void DrawMouseLine(){
      Handles.BeginGUI();
      Handles.color = Color.black;

      if (sourceNode != null) {
        var direction = (Event.current.mousePosition - sourceNode.GetCenter).normalized;
        Handles.DrawAAPolyLine(lineTexture, 3, sourceNode.GetCenter, Event.current.mousePosition);
      }

      Handles.EndGUI();
    }

    private void DrawLinkLines() {
      Handles.BeginGUI();

      foreach (var node in metaData.TransitionNodes) {
        if (selectedNode == node){
          Handles.color = Color.magenta;
        } else {
          Handles.color = Color.black;
        }

        Handles.DrawAAPolyLine(lineTexture, 2, node.CenterSource, node.CenterDestination);
        Handles.DrawAAPolyLine(lineTexture, 2, node.ArrowLeftEnd, node.Center, node.ArrowRightEnd);
      }

      Handles.EndGUI();
    }

  }

  
}
