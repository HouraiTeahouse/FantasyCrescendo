using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace HouraiTeahouse {
  public class CCBuilderWindow : EditorWindow {
    private const float UpperTabHeight = 20;
    private readonly Vector2 NodeSize = new Vector2(120, 50);
    private readonly Vector2 NodeTextSize = new Vector2(60, 20);

    public bool initDone = false;
    public CCBuilderScriptableObject CCBuilder;
    public GUIStyle labelStyle;

    public Texture2D lineTexture;
    public Texture linkFreeTexture;
    public Texture linkFullTexture;

    public CCBuilderScriptableObject.CCNode LinkingCC = null;

    [MenuItem("Window/Character State Builder")]
    static void Init() {
      GetWindow(typeof(CCBuilderWindow)).Show();
    }

    private void OnEnable() {
      initDone = false;
      LinkingCC = null;
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

      linkFreeTexture = Resources.Load("CCBuilder/LinkButtonFree") as Texture;
      linkFullTexture = Resources.Load("CCBuilder/LinkButtonFull") as Texture;
    }

    private void OnGUI() {
      // Initialize
      if (CCBuilder == null)
        CCBuilder = CCBuilderScriptableObject.GetCCBuilder();

      if (!initDone)
        InitStyles();

      // Draw lines (underneath all)
      DrawLinkLines();

      // Draw top bar buttons
      GUILayout.BeginHorizontal(GUILayout.MaxHeight(UpperTabHeight));
      if (GUILayout.Button("Add Node")) {
        CCBuilder.AddNode();
      }
      if (GUILayout.Button("Build")) {
        //CCBuilder.UpdateFile();
      }
      GUILayout.EndHorizontal();

      // Draw each window
      BeginWindows();
      foreach (var item in CCBuilder.NodeList) {
        item.Window = GUI.Window(item.Id, new Rect(item.Window.position, NodeSize), DrawNode, "", GUIStyle.none);
      }
      EndWindows();

      HandleLinkInput();
      DrawMouseLine();

      EditorUtility.SetDirty(CCBuilder);
    }

    private void OnInspectorUpdate() {
      if (LinkingCC != null)
        Repaint();
    }

    private void DrawNode(int id) {
      var item = CCBuilder.NodeDictionary[id];
      var e = Event.current;

      // Reposition so it doesn't get too out of screen
      item.Window.position = new Vector2(Mathf.Max(item.Window.position.x, 0), Mathf.Max(item.Window.position.y, UpperTabHeight));
      
      // If touched, switch to that node's scriptable object
      if (e.button == 0 && e.type == EventType.MouseDown){
        Selection.activeObject = CCBuilder;
      }

      // Draw the damn box and text
      GUILayout.BeginArea(new Rect(Vector2.zero, item.Window.size), new GUIStyle("Box"));
      var rect = new Rect((item.Window.size - NodeTextSize) / 2, NodeTextSize);
      GUI.Label(rect, new GUIContent(item.Content), labelStyle);
      GUILayout.EndArea();

      // If right clicking, don't even try to drag
      if (e.button != 1) {
        GUI.DragWindow();
      }
    }

    private void TryToConnectTwoNodes(CCBuilderScriptableObject.CCNode incomingLink) {
      if (incomingLink != null && LinkingCC != incomingLink && !LinkingCC.Links.Contains(incomingLink.Id)) {
        LinkingCC.AddLink(incomingLink.Id);
      }
      LinkingCC = null;
      Repaint();
    }

    private void HandleLinkInput(){
      Event e = Event.current;
      if (e.button == 1) {
        switch (e.type) {
          case EventType.MouseDown:
            foreach (var item in CCBuilder.NodeList) {
              if (item.Window.Contains(e.mousePosition)) {
                LinkingCC = item;
                break;
              }
            }
            break;
          case EventType.MouseUp:
            if (LinkingCC == null) break;
            CCBuilderScriptableObject.CCNode temp = null;
            foreach (var item in CCBuilder.NodeList) {
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

      if (LinkingCC != null) {
        var direction = (Event.current.mousePosition - LinkingCC.GetCenter).normalized;
        Handles.DrawAAPolyLine(lineTexture, 3, LinkingCC.GetDrawLineEnd(direction), Event.current.mousePosition);
      }

      Handles.EndGUI();
    }

    private void DrawLinkLines() {
      Handles.BeginGUI();
      Handles.color = Color.black;

      foreach (var item in CCBuilder.NodeList) {
        foreach (var link in item.Links.Select(c => CCBuilder.NodeDictionary[c])) {
          var direction = (link.GetCenter - item.GetCenter).normalized;
          var startPoint = item.GetDrawLineEnd(direction);
          var endPoint = link.GetDrawLineEnd(direction);
          var midPoint = (endPoint + startPoint) / 2;
          Handles.DrawAAPolyLine(lineTexture, 2, startPoint, endPoint);
          Handles.DrawAAPolyLine(lineTexture, 2, item.GetDrawLineArrowEnd(direction, midPoint, Vector3.forward), midPoint,
                                                  item.GetDrawLineArrowEnd(direction, midPoint, Vector3.back));
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
