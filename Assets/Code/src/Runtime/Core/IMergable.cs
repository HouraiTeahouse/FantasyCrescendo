using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A generic interface for supporting merging with instances 
/// of type <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">the type that can be merged with.</typeparam>
public interface IMergable<T> {

  /// <summary>
  /// Merges the current instance with the provided object.
  /// </summary>
  /// <param name="obj">the object to merge with.</param>
  /// <returns>the merged object</returns>
  T MergeWith(T obj);

}

}