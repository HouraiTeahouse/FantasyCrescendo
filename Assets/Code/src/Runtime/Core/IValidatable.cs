using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// An object with potentially invalid states.
/// </summary>
public interface IValidatable {

  /// <summary>
  /// Gets whether the current state of the object is valid or not.
  /// </summary>
  bool IsValid { get; }

}

public static class IValidatableExtensions {

  /// <summary>
  /// Gets whether all of a given validatable set are valid or not.
  /// </summary>
  /// <param name="group">the objects to validate.</param>
  /// <returns>true, if all objects are valid, false otherwise.</returns>
  public static bool IsAllValid<T>(this IEnumerable<T> group) where T : IValidatable {
    if (group == null) {
      return false;
    }
    return group.All(v => v.IsValid);
  }

}

}
