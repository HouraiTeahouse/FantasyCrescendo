using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

public class HitboxEditorWindow : LockableEditorWindow {

  EditorTable<SerializedObject> table;
  GameObject[] _roots;
  HurtboxType hurtboxType;

  [MenuItem("Window/Hitbox Window")]
  public static void ShowHitboxWindow() {
    EditorWindow.GetWindow<HitboxEditorWindow>("Hitbox").Show();
  }

  EditorTable<SerializedObject>.Column CreatePropertyColumn(string propertyName, bool onlyOffensive = true) {
    return table.AddColumn((rect, serializedObject) => {
      var property = serializedObject.FindProperty(propertyName);
      if (property == null) return;
      EditorGUI.PropertyField(rect, property, GUIContent.none);
    });
  }

  void BuildTable() {
    table = new EditorTable<SerializedObject>();

    table.OnDrawRowStart += (rect, element) => {
      var gameObj = (element.targetObject as Component).gameObject;
      if (!Selection.gameObjects.Contains(gameObj)) return;
      var color = GUI.backgroundColor;
      GUI.backgroundColor = Color.blue;
      GUI.Box(rect, GUIContent.none);
      GUI.backgroundColor = color;
    };

    var enabledCol = CreatePropertyColumn("m_Enabled", false);
    var nameCol = table.AddColumn((rect, serializedObject) => {
      var target = serializedObject.targetObject as Component;
      EditorGUI.LabelField(rect, target.name);
      var evt = Event.current;
      var gameObj = target.gameObject;
      if (evt.type != EventType.MouseUp || !rect.Contains(evt.mousePosition)) return;
      if ((evt.modifiers & EventModifiers.Control) != 0) {
        if (Selection.gameObjects.Contains(gameObj)) return;
        var temp = new List<Object>(Selection.gameObjects);
        temp.Add(gameObj);
        Selection.objects = temp.ToArray();
      } else {
        Selection.activeGameObject = gameObj;
      }
    });
    var typeCol = table.AddColumn((rect, serializedObject) => {
      var color = GUI.color;
      var hitbox = serializedObject.targetObject as Hitbox;
      var hurtbox = serializedObject.targetObject as Hurtbox;
      if (hitbox != null) {
        GUI.color = HitboxUtil.GetHitboxColor(hitbox.Type);
      }
      if (hurtbox != null) {
        GUI.color = HitboxUtil.GetHurtboxColor(hurtbox.Type);
      }
      EditorGUI.PropertyField(rect, serializedObject.FindProperty("Type"), GUIContent.none);
      GUI.color = color;
    });
    var priorityCol = CreatePropertyColumn("Priority");
    var damageCol = CreatePropertyColumn("BaseDamage");
    var baseKnockbackCol = CreatePropertyColumn("BaseKnockback");
    var knockbackScalingCol = CreatePropertyColumn("KnockbackScaling");
    var angleCol = CreatePropertyColumn("KnockbackAngle");
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
  }

  void OnSelectionChange() {
      if (IsLocked) return;
      _roots = Selection.gameObjects.Select(g => g.transform.root.gameObject).Distinct().ToArray();
      Repaint();
  }

  IEnumerable<SerializedObject> GetAllChildren<T>() where T : Component {
    return _roots.SelectMany(g => g.GetComponentsInChildren<T>(true))
                .OrderByDescending(h => h.name)
                .Distinct()
                .Select(h => new SerializedObject(h));
  }

  void SetAllHurtboxTypes(HurtboxType type) {
    var serializedObjs = GetAllChildren<Hurtbox>();
    Undo.IncrementCurrentGroup();
    var hurtboxes = serializedObjs.Select(obj => obj.targetObject as Hurtbox); 
    var objs = hurtboxes.Cast<Object>().ToArray();
    Undo.RecordObjects(objs, "Change Hurtbox Type");
    foreach (var hurtbox in hurtboxes) {
      hurtbox.Type = type;
    }
    Undo.FlushUndoRecordObjects();
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
    pos.height -= 16f;

    _roots = _roots ?? new GameObject[0];

    table.Draw(pos, GetAllChildren<Hitbox>().Concat(GetAllChildren<Hurtbox>()));

    pos.y = pos.y + pos.height;
    pos.height = 16f;
    pos.width -= 200;

    hurtboxType = (HurtboxType)EditorGUI.EnumPopup(pos, hurtboxType);

    pos.x += pos.width;
    pos.width = 200;

    if (GUI.Button(pos, "Set Hurtbox Type")) {
      SetAllHurtboxTypes(hurtboxType);
    }

    // Force Repaint the animation view if something changed.
    if (GUI.changed) {
      InternalEditorUtility.RepaintAllViews();
    }
  }

  void Update() => Repaint();

}

}