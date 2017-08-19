using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using HouraiTeahouse.Editor;
using UnityEditorInternal;

namespace HouraiTeahouse.SmashBrew {

    public class HitboxEditorWindow : LockableEditorWindow {

        EditorTable<SerializedObject> table;
        GameObject[] _roots;

        [MenuItem("Window/SmashBrew Hitbox Window")]
        public static void ShowHitboxWindow() {
            EditorWindow.GetWindow<HitboxEditorWindow>("Hitbox").Show();
        }

        EditorTable<SerializedObject>.Column CreatePropertyColumn(string propertyName, bool onlyOffensive = true) {
            return table.AddColumn((rect, serializedObject) => {
                var hitbox = serializedObject.targetObject as Hitbox;
                if (hitbox.CurrentType == Hitbox.Type.Offensive || !onlyOffensive)
                    EditorGUI.PropertyField(rect, serializedObject.FindProperty(propertyName), GUIContent.none);
            });
        }

        void BuildTable() {
            table = new EditorTable<SerializedObject>();
            var enabledCol = CreatePropertyColumn("m_Enabled", false);
            var nameCol = table.AddColumn((rect, serializedObject) => {
                EditorGUI.LabelField(rect, serializedObject.targetObject.name);
            });
            var typeCol = table.AddColumn((rect, serializedObject) => {
                var hitbox = serializedObject.targetObject as Hitbox;
                var type = hitbox.CurrentType;
                var color = GUI.color;
                GUI.color = Config.Debug.GetHitboxColor(type);
                hitbox.CurrentType = (Hitbox.Type)EditorGUI.EnumPopup(rect, GUIContent.none, type);
                GUI.color = color;
            });
            var damageCol = CreatePropertyColumn("_damage");
            var baseKnockbackCol = CreatePropertyColumn("_baseKnockback");
            var knockbackScalingCol = CreatePropertyColumn("_knockbackScaling");
            var priorityCol = CreatePropertyColumn("_priority");
            var angleCol = CreatePropertyColumn("_angle");
            var reflectCol = CreatePropertyColumn("_reflectable");
            var absorbCol = CreatePropertyColumn("_absorbable");
            table.Padding = new Vector2(5f, 2f);
            table.LabelStyle = EditorStyles.toolbar;
            table.LabelPadding = 2.5f;
            enabledCol.Name = "";
            enabledCol.Width = 0.01f;
            nameCol.Name = "Hitbox";
            nameCol.Width = 0.19f;
            typeCol.Name = "Type";
            typeCol.Width = 0.1f;
            damageCol.Name = "Damage";
            damageCol.Width = 0.1f;
            baseKnockbackCol.Name = "Knockback";
            baseKnockbackCol.Width = 0.1f;
            knockbackScalingCol.Name = "Scaling";
            knockbackScalingCol.Width = 0.1f;
            priorityCol.Name = "Priority";
            priorityCol.Width = 0.1f;
            angleCol.Name = "Angle";
            angleCol.Width = 0.2f;
            reflectCol.Name = "ʞ";
            reflectCol.Width = 0.02f;
            absorbCol.Name = "->Ɔ";
            absorbCol.Width = 0.05f;
        }

        void OnSelectionChange() {
            if (IsLocked)
                return;
            _roots = Selection.gameObjects.Select(g => g.transform.root.gameObject).Distinct().ToArray();
            Repaint();
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// This function can be called multiple times per frame (one call per event).
        /// </summary>
        void OnGUI() {
            if (table == null)
                BuildTable();
            var pos = position;
            pos.x = 0f;
            pos.y = 0f;
            table.Draw(pos, _roots.EmptyIfNull().SelectMany(g => g.GetComponentsInChildren<Hitbox>(true))
                                             .Distinct()
                                             .OrderByDescending(h => h.name)
                                             .Select(h => new SerializedObject(h)));
            // Force Repaint the animation view if something changed.
            if (GUI.changed)
                InternalEditorUtility.RepaintAllViews();
        }

        void Update() {
            Repaint();
        }

    }

}