using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Crescendo.API.Editor {

    /// <summary>
    /// Creates a EditorWindow that has the small padlock button at the top
    /// like the Inspector.
    /// 
    /// The state of the padlock can be accessed through the Locked property.
    /// </summary>
    public abstract class LockableEditorWindow : EditorWindow, IHasCustomMenu {

        [System.NonSerialized]
        GUIStyle lockButtonStyle;

        [System.NonSerialized]
        bool locked = false;

        public bool IsLocked {
            get { return locked; }
            set { locked = value; }
        }

        void ShowButton(Rect position) {
            if (lockButtonStyle == null)
                lockButtonStyle = "IN LockButton";
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
        }
        
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu) {
            menu.AddItem(new GUIContent("Lock"), locked, () => {
                locked = !locked;
            });
        }
    }
}