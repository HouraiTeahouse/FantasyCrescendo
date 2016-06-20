using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace HouraiTeahouse.Editor {

    [InitializeOnLoad]
    public class QuickToggle {
        const string PrefKeyShowToggle = "UnityToolbag.QuickToggle.Visible";

        static GUIStyle styleLock, styleLockUnselected, styleVisible;

        static QuickToggle() {
            if (EditorPrefs.HasKey(PrefKeyShowToggle) == false) 
                EditorPrefs.SetBool(PrefKeyShowToggle, false);

            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyItem;
            EditorApplication.RepaintHierarchyWindow();
        }

        static void DrawHierarchyItem(int instanceId, Rect selectionRect) {
            BuildStyles();
            var target = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (target == null)
                return;

            // Reserve the draw rects
            var visRect = new Rect(selectionRect) {
                xMin = selectionRect.xMax - (selectionRect.height * 2.1f),
                xMax = selectionRect.xMax - selectionRect.height
            };
            var lockRect = new Rect(selectionRect) {
                xMin = selectionRect.xMax - selectionRect.height
            };

            // Draw the visibility toggle
            bool isActive = target.activeSelf;
            if (isActive != GUI.Toggle(visRect, isActive, GUIContent.none, styleVisible)) {
                SetVisible(target, !isActive);
                EditorApplication.RepaintHierarchyWindow();
            }

            // Draw lock toggle
            bool isLocked = (target.hideFlags & HideFlags.NotEditable) > 0;
            // Decide which GUIStyle to use for the button
            // If this item is currently selected, show the visible lock style, if not, invisible lock style
            GUIStyle lockStyle = (Selection.activeInstanceID == instanceId) ? styleLock : styleLockUnselected;
            if (isLocked != GUI.Toggle(lockRect, isLocked, GUIContent.none, lockStyle)) {
                SetLockObject(target, !isLocked);
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        static Object[] GatherObjects(GameObject root) {
            var objects = new List<Object>();
            var recurseStack = new Stack<GameObject>(new GameObject[] { root });

            while (recurseStack.Count > 0) {
                GameObject obj = recurseStack.Pop();
                objects.Add(obj);

                foreach (Transform childT in obj.transform)
                    recurseStack.Push(childT.gameObject);
            }
            return objects.ToArray();
        }

        static void SetLockObject(GameObject target, bool isLocked) {
            Object[] objects = GatherObjects(target);
            string undoString = string.Format("{0} {1}", isLocked ? "Lock" : "Unlock", target.name);
            Undo.RecordObjects(objects, undoString);

            foreach (Object obj in objects) {
                var go = obj as GameObject;

                // Set state according to isLocked
                if (isLocked)
                    go.hideFlags |= HideFlags.NotEditable;
                else
                    go.hideFlags &= ~HideFlags.NotEditable;

                // Set hideflags of components
                foreach (Component comp in go.GetComponents<Component>()) {
                    if (comp is Transform)
                        continue;

                    if (isLocked) {
                        comp.hideFlags |= HideFlags.NotEditable;
                        comp.hideFlags |= HideFlags.HideInHierarchy;
                    } else {
                        comp.hideFlags &= ~HideFlags.NotEditable;
                        comp.hideFlags &= ~HideFlags.HideInHierarchy;
                    }
                    EditorUtility.SetDirty(comp);
                }
                EditorUtility.SetDirty(obj);
            }
        }

        static void SetVisible(GameObject target, bool isActive) {
            string undoString = string.Format("{0} {1}",
                                        isActive ? "Show" : "Hide",
                                        target.name);
            Undo.RecordObject(target, undoString);

            target.SetActive(isActive);
            EditorUtility.SetDirty(target);
        }

        static void BuildStyles() {
            // All of the styles have been built, don't do anything
            if (styleLock != null &&
                styleLockUnselected != null &&
                styleVisible != null) {
                return;
            }

            // First, get the textures for the GUIStyles
            Texture2D icnLockOn = null,
                    icnLockOnActive = null;
            var normalPassed = false;
            var activePassed = false;

            // Resource name of icon images
            const string resLockActive = "IN LockButton on";
            const string resLockOn = "IN LockButton on act";

            // Loop through all of the icons inside Resources
            // which contains editor UI textures
            Texture2D[] resTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D resTexture in resTextures) {
                // Regular icon
                if (resTexture.name.Equals(resLockOn)) {
                    // if not using pro skin, use the first 'IN LockButton on'
                    // that is passed when iterating
                    if (!EditorGUIUtility.isProSkin && !normalPassed)
                        icnLockOn = resTexture;
                    else
                        icnLockOn = resTexture;
                    normalPassed = true;
                }

                // active icon
                if (!resTexture.name.Equals(resLockActive)) continue;
                if (!EditorGUIUtility.isProSkin && !activePassed)
                    icnLockOnActive = resTexture;
                else
                    icnLockOnActive = resTexture;
                activePassed = true;
            }

            // Now build the GUI styles
            // Using icons different from regular lock button so that
            // it would look darker
            styleLock = new GUIStyle(GUI.skin.FindStyle("IN LockButton")) {
                onNormal = new GUIStyleState() { background = icnLockOn },
                onHover = new GUIStyleState() { background = icnLockOn },
                onFocused = new GUIStyleState() { background = icnLockOn },
                onActive = new GUIStyleState() { background = icnLockOnActive },
            };

            // Unselected just makes the normal states have no lock images
            GUIStyle tempStyle = GUI.skin.FindStyle("OL Toggle");
            styleLockUnselected = new GUIStyle(styleLock) {
                normal = tempStyle.normal,
                active = tempStyle.active,
                hover = tempStyle.hover,
                focused = tempStyle.focused
            };

            styleVisible = new GUIStyle(GUI.skin.FindStyle("VisibilityToggle"));
        }
    }
}
