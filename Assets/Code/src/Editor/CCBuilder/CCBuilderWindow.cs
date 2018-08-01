using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse {
  public class CCBuilderWindow : EditorWindow {
    private const float UpperTabHeight = 20;
    public CCBuilderScriptableObject CCBuilder;
    public GUIStyle textAreaStyle;

    [MenuItem("Window/Character State Builder")]
    static void Init() {
      GetWindow(typeof(CCBuilderWindow)).Show();
    }

    private void OnGUI() {
      if (CCBuilder == null)
        CCBuilder = CCBuilderScriptableObject.GetCCBuilder();

      textAreaStyle = new GUIStyle(GUI.skin.textArea);
      textAreaStyle.wordWrap = true;

      GUILayout.BeginHorizontal(GUILayout.MaxHeight(UpperTabHeight));
      if (GUILayout.Button("Add Character State")){
        CCBuilder.AddCharacterState();
      }
      if (GUILayout.Button("Add Function")) {
        CCBuilder.AddFunction();
      }
      GUILayout.EndHorizontal();

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
    }

    private void OnCharacterStateWindow(int id){
      var item = (CCBuilderScriptableObject.CCCharacterStates)CCBuilder.CCDictionary[id];      

      var textHeight = textAreaStyle.CalcHeight(new GUIContent(item.CharacterStates), 150);
      item.CharacterStates = TextArea(item.CharacterStates, textAreaStyle, GUILayout.Width(150), GUILayout.Height(textHeight));

      GUI.DragWindow();
    }

    private void OnFunctionWindow(int id) {
      var item = (CCBuilderScriptableObject.CCFunction)CCBuilder.CCDictionary[id];

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

      GUI.DragWindow();
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
