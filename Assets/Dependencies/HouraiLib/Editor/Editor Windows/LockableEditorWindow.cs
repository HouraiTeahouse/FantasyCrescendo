using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Creates a EditorWindow that has the small padlock button at the top
    /// like the Inspector.
    /// 
    /// The state of the padlock can be accessed through the Locked property.
    /// </summary>
    public abstract class LockableEditorWindow : EditorWindow, IHasCustomMenu {

        private GUIStyle lockButtonStyle;
        private bool locked;

        /// <summary>
        /// Whether the EditorWindow is currently locked or not.
        /// </summary>
        public bool IsLocked {
            get { return locked; }
            set { locked = value; }
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu) {
            menu.AddItem(new GUIContent("Lock"), locked, () => { locked = !locked; });
        }

        void ShowButton(Rect position) {
            if (lockButtonStyle == null)
                lockButtonStyle = "IN LockButton";
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
        }

    }

}
