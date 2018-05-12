using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary> 
/// Abstract base class for EditorWindows that has the small padlock button at 
/// the top like the Inspector. The state of the /// padlock can be accessed 
/// through the IsLocked property. 
/// </summary>
public abstract class LockableEditorWindow : EditorWindow, IHasCustomMenu {

  GUIStyle lockButtonStyle;

  /// <summary> 
  /// Gets or sets whether the EditorWindow is currently locked or not. 
  /// </summary>
  public bool IsLocked { get; set; }

  public virtual void AddItemsToMenu(GenericMenu menu) {
    menu.AddItem(new GUIContent("Lock"), IsLocked, () => { IsLocked = !IsLocked; });
  }

  void ShowButton(Rect position) {
    if (lockButtonStyle == null)
      lockButtonStyle = "IN LockButton";
    IsLocked = GUI.Toggle(position, IsLocked, GUIContent.none, lockButtonStyle);
  }

}

}