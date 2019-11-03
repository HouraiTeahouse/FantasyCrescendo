using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.FantasyCrescendo.Characters;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

class EditorCommands {

    [MenuItem("Hitbox/Add Hurtbox %#h")]
    static void AddHurtbox() => AddHitbox<Hurtbox>("Damageable");

    static T CreateHitbox<T>(Transform parent) where T : Component {
      var hbGo = new GameObject();
      Undo.RegisterCreatedObjectUndo(hbGo, "Create Hitbox GameObject");
      var hb = Undo.AddComponent<T>(hbGo);
      if (parent != null) {
        Undo.SetTransformParent(hb.transform, parent, "Parent Hitbox");
      }

      // Reset hitboxes local transform
      hb.transform.localPosition = Vector3.zero;
      hb.transform.localScale = Vector3.one;
      hb.transform.localRotation = Quaternion.identity;

      if (hb is Hurtbox) {
        hb.gameObject.AddComponent<SphereCollider>();
      }

      return hb;
    }

    static void AddHitboxToSet<T>(Dictionary<GameObject, List<T>> rootMap, GameObject rootGo, T hitbox) {
        if (!rootMap.ContainsKey(rootGo))
            rootMap[rootGo] = new List<T>();
        rootMap[rootGo].Add(hitbox);
    }

    static void AddHitbox<T>(string type) where T : Component {
      var hitboxes = new List<T>();
      Undo.IncrementCurrentGroup();
      if (Selection.gameObjects.Length <= 0)  {
        var hitbox = CreateHitbox<T>(null);
        hitboxes.Add(hitbox);
        hitbox.name = $"hb_{type}";
      } else {
        var rootMap = new Dictionary<GameObject, List<T>>();
        foreach (GameObject gameObject in Selection.gameObjects) {
          var hitbox = CreateHitbox<T>(gameObject.transform);
          hitboxes.Add(hitbox);
          AddHitboxToSet<T>(rootMap, hitbox.transform.root.gameObject, hitbox);
        }
        foreach (KeyValuePair<GameObject, List<T>> set in rootMap) {
          T[] allHitboxes = set.Key.GetComponentsInChildren<T>();
          int i = allHitboxes.Length - set.Value.Count;
          Undo.RecordObjects(set.Value.ToArray(), "Name Changes");
          foreach (T hitbox in set.Value) {
            hitbox.name = $"{set.Key.name}_hb_{type}_{i}";
            i++;
          }
        }
      }
      Selection.objects = hitboxes.Select(h => h.gameObject).ToArray();
      string suffix = (hitboxes.Count > 0) ? "es" : string.Empty;
      Undo.SetCurrentGroupName($"Generate {type} Hitbox{suffix}");
    }

}

}
