using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using HouraiTeahouse.Editor;

namespace HouraiTeahouse.SmashBrew {

    public class HitboxEditorWindow : LockableEditorWindow {

        EditorTable<Hitbox> table;
        GameObject[] _roots;

        [MenuItem("Window/SmashBrew Hitbox Window")]
        public static void ShowHitboxWindow() {
            EditorWindow.GetWindow<HitboxEditorWindow>("Hitbox").Show();
        }

        void BuildTable() {
            table = new EditorTable<Hitbox>();
            var nameCol = table.AddColumn((rect, hitbox) => {
                hitbox.enabled = EditorGUI.ToggleLeft(rect, hitbox.name, hitbox.enabled);
            });
            var typeCol = table.AddColumn((rect, hitbox) => {
                var type = hitbox.CurrentType;
                var color = GUI.color;
                GUI.color = Config.Debug.GetHitboxColor(type);
                hitbox.CurrentType = (Hitbox.Type)EditorGUI.EnumPopup(rect, GUIContent.none, type);
                GUI.color = color;
            });
            var damageCol = table.AddColumn((rect, hitbox) => {
                if (hitbox.CurrentType == Hitbox.Type.Offensive)
                    hitbox.Priority = EditorGUI.IntField(rect, GUIContent.none, hitbox.Priority);
            });
            var baseKnockbackCol = table.AddColumn((rect, hitbox) => {
                if (hitbox.CurrentType == Hitbox.Type.Offensive)
                    hitbox.BaseKnockback = EditorGUI.FloatField(rect, GUIContent.none, hitbox.BaseKnockback);
            });
            var knockbackScalingCol = table.AddColumn((rect, hitbox) => {
                if (hitbox.CurrentType == Hitbox.Type.Offensive)
                    hitbox.Scaling = EditorGUI.FloatField(rect, GUIContent.none, hitbox.Scaling);
            });
            var priorityCol = table.AddColumn((rect, hitbox) => {
                if (hitbox.CurrentType == Hitbox.Type.Offensive)
                    hitbox.Priority = EditorGUI.IntField(rect, GUIContent.none, hitbox.Priority);
            });
            var angleCol = table.AddColumn((rect, hitbox) => {
                if (hitbox.CurrentType == Hitbox.Type.Offensive)
                    hitbox.Angle = EditorGUI.Slider(rect, GUIContent.none, hitbox.Angle, 0f, 360f);
            });
            var reflectCol = table.AddColumn((rect, hitbox) => {
                if (hitbox.CurrentType == Hitbox.Type.Offensive)
                    hitbox.Reflectable = EditorGUI.ToggleLeft(rect, GUIContent.none, hitbox.Reflectable);
            });
            var absorbCol = table.AddColumn((rect, hitbox) => {
                if (hitbox.CurrentType == Hitbox.Type.Offensive)
                    hitbox.Reflectable = EditorGUI.ToggleLeft(rect, GUIContent.none, hitbox.Reflectable);
            });
            table.Padding = new Vector2(5f, 2f);
            table.LabelStyle = EditorStyles.toolbar;
            table.LabelPadding = 2.5f;
            nameCol.Name = "Hitbox";
            nameCol.Width = 0.2f;
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
                                             .OrderByDescending(h => h.name));
        }

    }

}