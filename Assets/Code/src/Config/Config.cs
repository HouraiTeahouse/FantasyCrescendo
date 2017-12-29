using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

public static class Config {

  static Dictionary<Type, ScriptableObject> configs;

  static Config() {
    configs = new Dictionary<Type, ScriptableObject>();
  }

  public static void Register(ScriptableObject config) {
    configs[config.GetType()] = config;
  }

  public static T Get<T>() where T : ScriptableObject {
    ScriptableObject config;
    if (!configs.TryGetValue(typeof(T), out config)) {
      config = ScriptableObject.CreateInstance<T>();
      configs[typeof(T)] = config;
    }
    return config as T;
  }

  public static void Clear() {
    foreach (var kvp in configs) {
      Object.Destroy(kvp.Value);
    }
    configs.Clear();
  }

}

}
