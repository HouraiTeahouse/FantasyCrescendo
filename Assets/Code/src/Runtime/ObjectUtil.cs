using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public static class ObjectUtil {

  public static void SetActive(Object obj, bool active) {
    if (obj == null) return;
    var gameObject = obj as GameObject;
    var behavior = obj as Behaviour;
    if (gameObject != null && gameObject.activeSelf != active ) {
      gameObject.SetActive(active);
    }
    if (behavior != null) {
      behavior.enabled = active;
    }
  }

  public static T ForceNull<T>(this T obj) where T : Object {
    return obj == null ? null : obj;
  }

}

}