using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    [InitializeOnLoad]
    public class QuickToggle {

        const string PrefKeyShowToggle = "UnityToolbag.QuickToggle.Visible";

        static GUIStyle styleLock,
                        styleVisible;

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
                xMin = selectionRect.xMax - selectionRect.height * 2.1f,
                xMax = selectionRect.xMax - selectionRect.height
            };
            var lockRect = new Rect(selectionRect) {xMin = selectionRect.xMax - selectionRect.height};

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
            if (isLocked != GUI.Toggle(lockRect, isLocked, GUIContent.none, styleLock)) {
                SetLockObject(target, !isLocked);
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        static Object[] GatherObjects(GameObject root) {
            var objects = new List<Object>();
            var recurseStack = new Stack<GameObject>(new[] {root});

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
                    }
                    else {
                        comp.hideFlags &= ~HideFlags.NotEditable;
                        comp.hideFlags &= ~HideFlags.HideInHierarchy;
                    }
                    EditorUtility.SetDirty(comp);
                }
                EditorUtility.SetDirty(obj);
            }
        }

        static void SetVisible(GameObject target, bool isActive) {
            string undoString = string.Format("{0} {1}", isActive ? "Show" : "Hide", target.name);
            Undo.RecordObject(target, undoString);

            target.SetActive(isActive);
            EditorUtility.SetDirty(target);
        }

        static void BuildStyles() {
            // All of the styles have been built, don't do anything
            if (styleLock != null && styleVisible != null) {
                return;
            }

            // Now build the GUI styles
            // Using icons different from regular lock button so that
            // it would look darker
            styleLock = GUI.skin.FindStyle("IN LockButton");
            styleVisible = GUI.skin.FindStyle("VisibilityToggle");
        }

    }

}