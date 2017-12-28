using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

public static class PlayerUtil {

  public static void DestroyAll(GameObject characterObject,
                                params Type[] componentTypes) {
    foreach (var type in componentTypes) {
      foreach (var component in characterObject.GetComponentsInChildren(type)) {
        Object.DestroyImmediate(component);
      }
    }
  }

}

}
