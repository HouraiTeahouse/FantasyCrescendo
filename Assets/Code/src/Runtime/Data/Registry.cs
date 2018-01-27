using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A type-safe global object registry.
/// </summary>
/// <remarks> 
/// Only supports types that implement IIdentifiable.
/// Objects are referred to by their IDs, and the ID for a given
/// object is assumed to be globally unique.
/// </remarks>
public static class Registry {

  static Dictionary<Type, object> Registries;

  static Registry() {
    Registries = new Dictionary<Type, object>();
  }

  /// <summary>
  /// Retrieves or creates a new registry for given type.
  /// </summary>
  /// <returns>the retrieved registry for the type.</returns>
  public static Registry<T> Get<T>() where T : IEntity {
    object storedObject = null;
    Registries.TryGetValue(typeof(T), out storedObject);
    var registry = storedObject as Registry<T>;
    if (registry == null) {
      registry = new Registry<T>();
      Registries.Add(typeof(T), registry);
    }
    return registry;
  }

  public static void Register(Type targetType, IEntity obj) {
    object registry = null;
    Registries.TryGetValue(targetType, out registry);
    if (registry == null) {
      var registryType = typeof(Registry<>).MakeGenericType(targetType);
      registry = Activator.CreateInstance(registryType);
      Registries.Add(targetType, registry);
    }
    var addMethod = registry.GetType().GetMethod("Add");
    addMethod.Invoke(registry, new [] { obj });
  }

  /// <summary>
  /// Clears all registries of all types and the values they store.
  /// </summary>
  public static void ClearAll() {
    Registries.Clear();
  }

}

public class Registry<T> : ICollection<T> where T : IEntity {

  readonly Dictionary<uint, T> Entries;

  public Registry() {
    Entries = new Dictionary<uint, T>();
  }

  public int Count => Entries.Count; 

  public bool IsReadOnly => false; 

  public T Get(uint id) {
    T obj;
    if (Entries.TryGetValue(id, out obj)) {
      return obj;
    }
    return default(T);
  }

  public void Add(T obj) => Entries[obj.Id] = obj;

  public bool Remove(T obj) => Entries.Remove(obj.Id);

  public bool Contains(T obj) => Entries.ContainsKey(obj.Id);

  public void Clear() => Entries.Clear();

  public void CopyTo(T[] array, int start) => Entries.Values.CopyTo(array, start);

  public IEnumerator<T> GetEnumerator() => Entries.Values.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); 

}

}

