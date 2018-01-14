using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public static class Messaging {

  public static T[] Broadcast<T>(this GameObject gameObject, Action<T> message) {
    T[] components = gameObject.GetComponentsInChildren<T>();
    foreach (var component in components) {
      message(component);
    }
    return components;
  }

  public static Task Broadcast<T>(this GameObject gameObject, Func<T, Task> message) {
    return Task.WhenAll(gameObject.GetComponentsInChildren<T>().Select(message));
  }

}

}
