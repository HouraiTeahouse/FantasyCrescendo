using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

using StateNode = HouraiTeahouse.FantasyCrescendo.Characters.StateMachineMetadata.StateNode;
using TransitionNode = HouraiTeahouse.FantasyCrescendo.Characters.StateMachineMetadata.TransitionNode;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateMachineBuilderWindow : LockableEditorWindow {
  // Window drawing purposes
  static Matrix4x4 _prevGuiMatrix;
  readonly float editorWindowHeight = 21;
  readonly float tabHeight = 17;
  readonly Vector2 displayArea = new Vector2(1600, 1200);
  readonly Vector2 NodeTextSize = new Vector2(60, 20);

  readonly float minZoomScale = 0.5f;
  readonly float maxZoomScale = 2.0f;

  // Other vars
  bool initDone = false;
  StateMachineMetadata metaData;
  StateNode sourceNode = null;

  // Styles and Textures
  GUIStyle labelStyle;
  GUIStyle nodeStyle;
  Texture2D lineTexture;

  // Grid
  Vector2 gridOffset;
  Vector2 gridDrag;

  [MenuItem("Window/State Machine Builder Window")]
  static void Init() {
    GetWindow(typeof(StateMachineBuilderWindow)).Show();
  }

  void OnEnable() {
    initDone = false;
    sourceNode = null;
  }

  void InitStyles() {
    initDone = true;

    labelStyle = GUI.skin.label;
    labelStyle.alignment = TextAnchor.MiddleCenter;
    labelStyle.richText = true;
    labelStyle.fontSize = 15;

    nodeStyle = new GUIStyle();
    nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
    nodeStyle.border = new RectOffset(12, 12, 12, 12);

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

    // Draw environment (inclusing zoom and position)
    BeginMainWindow(metaData.WindowOffset, metaData.WindowZoomPivot, metaData.WindowZoom);

    DrawGrid(displayArea, 20, 0.2f, Color.gray);
    DrawGrid(displayArea, 100, 0.4f, Color.gray);

    DrawLinkLines();

    BeginWindows();
    foreach (var item in metaData.StateNodes) {
      item.Center = GUI.Window(item.Id, item.Window, DrawNode, "", nodeStyle).center;
    }
    EndWindows();
    DrawMouseLine();

    EndMainWindow();

    // Handle input (mouse and keyboard)
    HandleEvents();

    // Draw UI (top bars and instructions)
    GUILayout.BeginVertical();
    DrawToolBar();
    GUILayout.FlexibleSpace();
    GUILayout.BeginHorizontal();
    GUILayout.Label(new GUIContent("Middle button to pan and zoom."));
    GUILayout.FlexibleSpace();
    GUILayout.Label(new GUIContent("Right click to create transitions"));
    GUILayout.EndHorizontal();
    GUILayout.EndVertical();

    EditorUtility.SetDirty(metaData);
  }

  void OnInspectorUpdate() {
    if (sourceNode != null)
      Repaint();
  }

  void BeginMainWindow(Vector2 offset, Vector2 pivot, float zoomScale) {
    GUI.EndGroup();
    var mainWindowArea = new Rect(offset * zoomScale, displayArea);
    mainWindowArea.y += tabHeight + editorWindowHeight;
    GUI.BeginGroup(mainWindowArea);

    _prevGuiMatrix = GUI.matrix;
    GUIUtility.ScaleAroundPivot(Vector2.one * zoomScale, pivot + offset);
  }

  void EndMainWindow() {
    GUI.matrix = _prevGuiMatrix;
    GUI.EndGroup();
    GUI.BeginGroup(new Rect(0, editorWindowHeight, Screen.width, Screen.height));
  }

  void AddNode(Vector2 position = default(Vector2)) {
    var node = metaData.AddStateNode();
    node.Center = position;
    SelectObject(node.Asset);
  }

  void DeleteNodes(IEnumerable<Object> objects) {
    bool deleted = false;
    foreach (var obj in objects) {
      var state = obj as StateAsset;
      var transition = obj as StateTransitionAsset;
      if (state != null) {
        var node = metaData.FindState(state);
        if (node != null) {
          metaData.RemoveStateNode(node);
          deleted = true;
        }
      } else if (transition != null) {
        var node = metaData.FindTransition(transition);
        if (node != null) {
          metaData.RemoveTransitionNode(node);
          deleted = true;
        }
      }
    }
    if (deleted) {
      Repaint();
    }
  }

  void DrawToolBar() {
    GUILayout.BeginHorizontal(EditorStyles.toolbar);
    EditorGUILayout.LabelField(metaData._stateMachine.name);
    GUILayout.EndHorizontal();
  }

  void DrawNode(int id) {
    StateNode node;
    if (!metaData.StateDictionary.TryGetValue(id, out node)) return;
    var e = Event.current;

    // Select once clicked
    if (e.button == 0 && e.type == EventType.MouseDown) {
      Selection.activeObject = node.Asset;
    }

    // Reposition box if out of bounds
    node.Center.x = Mathf.Clamp(node.Center.x, node.Window.size.x / 2, displayArea.x - node.Window.size.x / 2);
    node.Center.y = Mathf.Clamp(node.Center.y, node.Window.size.y / 2, displayArea.y - node.Window.size.y / 2);

    // Draw the damn box and text
    GUILayout.BeginArea(new Rect(Vector2.zero, node.Window.size));
    var rect = new Rect((node.Window.size - NodeTextSize) / 2, NodeTextSize);
    GUI.Label(rect, new GUIContent(node.GetRichText()), labelStyle);
    GUILayout.EndArea();

    // If left clicking, then drag
    if (e.button == 0) {
      GUI.DragWindow();
    }
  }

  void ProcessContextMenu(Vector2 mousePosition) {
    GenericMenu genericMenu = new GenericMenu();
    genericMenu.AddItem(new GUIContent("Add State"), false, () => AddNode(mousePosition));
    genericMenu.ShowAsContext();
  }

  void TryToConnectTwoNodes(StateNode destinationNode) {
    Object asset = null;
    if (destinationNode != null &&
        sourceNode != destinationNode &&
        !metaData.TransitionNodeExists(sourceNode, destinationNode)) {
        asset = metaData.AddTransitionNode(sourceNode, destinationNode).Asset;
    }
    sourceNode = null;
    SelectObject(asset);
  }

  void SelectObject(Object obj){
    Selection.activeObject = obj;
    Repaint();
  }

  void SelectObjects(Object[] objs) {
    Selection.objects = objs;
    Repaint();
  }


  void HandleEvents() {
    HandleMouseInput();
    HandleKeyboardInput();
  }

  void HandleKeyboardInput() {
    Event evt = Event.current;
    if (!evt.isKey) return;
    switch (evt.keyCode) {
      case KeyCode.Delete:
        DeleteNodes(Selection.objects);
        break;
      case KeyCode.Escape:
        SelectObject(null);
        break;
    }
  }

  void HandleMouseInput() {
    Event evt = Event.current;
    if (evt.mousePosition.y < tabHeight) return;
    var areaPosition = ScreenToZoomPosition(evt.mousePosition);
    switch (evt.button) {
      // Left Click
      case 0:
        // Finding transitions
        if (evt.type == EventType.MouseDown) {
          foreach (var item in metaData.TransitionNodes) {
            if (item.Contains(areaPosition)) {
              SelectObject(item.Asset);
              evt.Use();
              return;
            }
          }
        }
        break;
      // Right Click
      case 1:
        switch (evt.type) {
          // Start link
          case EventType.MouseDown:
            foreach (var item in metaData.StateNodes) {
              if (item.Window.Contains(areaPosition)) {
                sourceNode = item;
                evt.Use();
                return;
              }
            }
            // No node found, expose context menu
            ProcessContextMenu(areaPosition);
            break;
          // End link
          case EventType.MouseUp:
            if (sourceNode == null) break;
            StateNode temp = null;
            foreach (var item in metaData.StateNodes) {
              if (item.Window.Contains(areaPosition)) {
                temp = item;
                evt.Use();
                break;
              }
            }
            TryToConnectTwoNodes(temp);
            break;
        }
        break;
      // Scroll Wheel (Click and Dragging)
      case 2:
        if (evt.type == EventType.MouseDrag) {
          metaData.WindowOffset += evt.delta / metaData.WindowZoom;
          evt.Use();
        }
        break;
    }
    // Scroll Whell
    if (evt.type == EventType.ScrollWheel) {
      metaData.WindowZoomPivot = areaPosition;
      metaData.WindowZoom = Mathf.Clamp(metaData.WindowZoom + evt.delta.y / -25.0f, minZoomScale, maxZoomScale);
      metaData.WindowOffset += (ScreenToZoomPosition(evt.mousePosition) - areaPosition) * metaData.WindowZoom;
      evt.Use();
    }
  }

  // -------------------
  // Draw line functions
  // -------------------

  void DrawMouseLine() {
    Handles.BeginGUI();
    Handles.color = Color.black;

    if (sourceNode != null) {
      var direction = (Event.current.mousePosition - sourceNode.Center).normalized;
      Handles.DrawAAPolyLine(lineTexture, 3, sourceNode.Center, Event.current.mousePosition);
    }

    Handles.EndGUI();
  }

  void DrawLinkLines() {
    metaData.UpdateTransitionNodes();
    Handles.BeginGUI();

    foreach (var node in metaData.TransitionNodes) {
      if (node.IsSelected) {
        Handles.color = Color.magenta;
      } else {
        Handles.color = Color.black;
      }

      Handles.DrawAAPolyLine(lineTexture, 2, node.CenterSource, node.CenterDestination);
      Handles.DrawAAPolyLine(lineTexture, 2, node.ArrowLeftEnd, node.Center, node.ArrowRightEnd);
    }

    Handles.EndGUI();
  }

  void DrawGrid(Vector2 area, float gridSpacing, float gridOpacity, Color gridColor) {
    int widthDivs = Mathf.CeilToInt(area.x / gridSpacing);
    int heightDivs = Mathf.CeilToInt(area.y / gridSpacing);

    Handles.BeginGUI();
    Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

    gridOffset += gridDrag * 0.5f;
    Vector3 newOffset = new Vector3(gridOffset.x % gridSpacing, gridOffset.y % gridSpacing, 0);

    for (int i = 0; i < widthDivs; i++) {
      Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, area.y, 0f) + newOffset);
    }

    for (int j = 0; j < heightDivs; j++) {
      Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(area.x, gridSpacing * j, 0f) + newOffset);
    }

    Handles.color = Color.white;
    Handles.EndGUI();
  }

  /// <summary>
  /// Converts screen position (typically from event.current.mouseposition)
  /// to position of GUI.Group()
  /// </summary>
  /// <param name="screenPosition"></param>
  /// <returns>position of GUI.Group()</returns>
  private Vector2 ScreenToZoomPosition(Vector2 screenPosition)
    => (screenPosition - new Vector2(0, tabHeight) - metaData.WindowOffset) 
      / metaData.WindowZoom + (metaData.WindowZoomPivot - metaData.WindowZoomPivot / metaData.WindowZoom);
}

}
