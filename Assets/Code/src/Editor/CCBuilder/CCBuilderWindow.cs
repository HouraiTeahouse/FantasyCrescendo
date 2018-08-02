using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse {
  public class CCBuilderWindow : EditorWindow {
    private const float UpperTabHeight = 20;

    public bool updateAssets;

    public bool initDone = false;
    public CCBuilderScriptableObject CCBuilder;
    public GUIStyle textAreaStyle;
    public Material material;

    public Texture2D lineTexture;
    public Texture linkTexture;

    public CCBuilderScriptableObject.CCGeneral LinkingCC = null;

    [MenuItem("Window/Character State Builder")]
    static void Init() {
      GetWindow(typeof(CCBuilderWindow)).Show();
    }

    private void OnEnable() {
      initDone = false;
      LinkingCC = null;
    }

    private void InitStyles(){
      initDone = true;
    
      textAreaStyle = new GUIStyle(GUI.skin.textArea);
      textAreaStyle.wordWrap = true;

      material = new Material(Shader.Find("Hidden/Internal-Colored"));

      lineTexture = new Texture2D(1, 2);
      lineTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.5f));
      lineTexture.SetPixel(0, 1, Color.white);
      lineTexture.Apply();

      linkTexture = Resources.Load("CCBuilder/LinkButton") as Texture;
    }

    private void OnGUI() {
      // Initialize
      if (CCBuilder == null)
        CCBuilder = CCBuilderScriptableObject.GetCCBuilder();

      if (!initDone) 
        InitStyles();

      updateAssets = false;

      // Draw top bar buttons
      GUILayout.BeginHorizontal(GUILayout.MaxHeight(UpperTabHeight));
      if (GUILayout.Button("Add Character State")){
        CCBuilder.AddCharacterState();
        updateAssets = true;
      }
      if (GUILayout.Button("Add Function")) {
        CCBuilder.AddFunction();
      }
      if (GUILayout.Button("Build")) {
        CCBuilder.UpdateFile();
      }
      GUILayout.EndHorizontal();

      // Draw each window
      BeginWindows();
      foreach (var item in CCBuilder.GetCharacterStates) {
        item.Window = GUILayout.Window(item.Id, new Rect(item.Window.position, Vector2.zero), OnCharacterStateWindow, "Character States");
        item.Window.position = new Vector2(Mathf.Max(0, item.Window.position.x), Mathf.Max(UpperTabHeight, item.Window.position.y));
      }

      foreach (var item in CCBuilder.GetFunctions) {
        item.Window = GUILayout.Window(item.Id, new Rect(item.Window.position, Vector2.zero), OnFunctionWindow, "Function");
        item.Window.position = new Vector2(Mathf.Max(0, item.Window.position.x), Mathf.Max(UpperTabHeight, item.Window.position.y));
      }
      EndWindows();

      DrawLines();

      if (LinkingCC != null)
        Repaint();

      EditorUtility.SetDirty(CCBuilder);
    }

    private void OnCharacterStateWindow(int id){
      var item = (CCBuilderScriptableObject.CCCharacterStates)CCBuilder.CCDictionary[id];

      GUILayout.BeginHorizontal();
      var textHeight = textAreaStyle.CalcHeight(new GUIContent(item.CharacterStates), 150);
      item.CharacterStates = TextArea(item.CharacterStates, textAreaStyle, GUILayout.Width(150), GUILayout.Height(textHeight));

      if(GUILayout.Button(linkTexture, GUIStyle.none)){
        HandleLinkButton(item);
      }
      item.LinkRect = GUILayoutUtility.GetLastRect();

      GUILayout.EndHorizontal();

      GUI.DragWindow();
    }

    private void OnFunctionWindow(int id) {
      var item = (CCBuilderScriptableObject.CCFunction)CCBuilder.CCDictionary[id];

      GUILayout.BeginHorizontal();

      if (GUILayout.Button(linkTexture, GUIStyle.none)) {
        HandleLinkButton(item);
      }
      item.LinkRect = GUILayoutUtility.GetLastRect();

      GUILayout.BeginVertical();
      if (GUILayout.Button(item.IsCharacterStateMulti ? "Func<CharacterState, CharacterContext>" :
                                      "Func<CharacterState, bool>")) {
        item.IsCharacterStateMulti = !item.IsCharacterStateMulti;
      }

      if (!item.IsCharacterStateMulti){
        GUILayout.BeginHorizontal();
        GUILayout.Label("Character State");
        item.CharacterState = TextField(item.CharacterState, GUILayout.Width(125));
        GUILayout.EndHorizontal();
      }

      var textHeight = textAreaStyle.CalcHeight(new GUIContent(item.Function), 250);
      item.Function = TextArea(item.Function, textAreaStyle, GUILayout.Width(250), GUILayout.Height(textHeight));
      GUILayout.EndVertical();

      GUILayout.EndHorizontal();
      GUI.DragWindow();
    }

    private void HandleLinkButton(CCBuilderScriptableObject.CCGeneral incomingLink) {
      if (LinkingCC == null){
        LinkingCC = incomingLink;
      } else if (LinkingCC.Id == incomingLink.Id){
        LinkingCC = null;
      } else {
        CCBuilderScriptableObject.CCCharacterStates CStemp = null;
        CCBuilderScriptableObject.CCFunction FNtemp = null;
        if (LinkingCC is CCBuilderScriptableObject.CCCharacterStates) {
          CStemp = (CCBuilderScriptableObject.CCCharacterStates)LinkingCC;
          FNtemp = (CCBuilderScriptableObject.CCFunction)incomingLink;
        } else if (incomingLink is CCBuilderScriptableObject.CCCharacterStates) {
          CStemp = (CCBuilderScriptableObject.CCCharacterStates)incomingLink;
          FNtemp = (CCBuilderScriptableObject.CCFunction)LinkingCC;
        }

        if (CStemp != null){
          if (!CStemp.Links.Contains(FNtemp.Id)) {
            CStemp.Links.Add(FNtemp.Id);
            LinkingCC = null;
          }
        }
      }
    }

    private void DrawLines(){
      Handles.BeginGUI();
      Handles.color = Color.black;

      if (LinkingCC != null)
        Handles.DrawAAPolyLine(lineTexture, 5, LinkingCC.GetLinkCenter, Event.current.mousePosition);

      foreach (var item in CCBuilder.GetCharacterStates) {
        foreach (var linkItem in item.Links) {
          Handles.DrawAAPolyLine(lineTexture, 5, item.GetLinkCenter, CCBuilder.CCDictionary[linkItem].GetLinkCenter);
        }
      }

      Handles.EndGUI();
    }

    private void BuildScript(){
      
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
        }
        else if (e.type == EventType.ExecuteCommand){
          TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
          if (e.commandName == "Copy") {
            editor.Copy();
          } else if (e.commandName == "Paste") {
            editor.Paste();
            return editor.text;
          } else if (e.commandName == "SelectAll") {
            editor.SelectAll();
          } else if (e.commandName == "Cut"){
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
    public static string TextField(string value, params GUILayoutOption[] options) {
      int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
      if (textFieldID == 0)
        return value;

      // Handle custom copy-paste
      value = HandleCopyPaste(textFieldID) ?? value;

      return GUILayout.TextField(value, options);
    }

    /// <summary>
    /// TextField with copy-paste support
    /// </summary>
    public static string TextArea(string value, GUIStyle style, params GUILayoutOption[] options) {
      int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
      if (textFieldID == 0)
        return value;

      // Handle custom copy-paste
      value = HandleCopyPaste(textFieldID) ?? value;

      return GUILayout.TextArea(value, style, options);
    }
  }


}
