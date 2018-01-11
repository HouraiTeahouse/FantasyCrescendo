using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// Utility static class for management of Player related components.
/// </summary>
public static class PlayerUtil {

  /// <summary>
  /// Finds and destroys all components of a given set of types found on a
  /// GameObject or any of it's children.
  /// </summary>
  /// <remarks>
  /// This function uses Object.DestroyImmediate.
  /// </remarks>
  /// <param name="characterObject">the root object to search from.</param>
  /// <param name="componentTypes">the types to destroy.</param>
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
