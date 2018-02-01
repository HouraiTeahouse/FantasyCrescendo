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

/// <summary>
/// A validation policy to check the validity of the state of 
/// provided objects.
/// </summary>
/// <typeparam name="T">the type of object that can be validated.</typeparam>
public interface IValidator<T> {

  /// <summary>
  /// Gets whether <paramref cref="obj"/> is valid or not.
  /// </summary>
  /// <param name="obj">the object to validate.</param>
  /// <returns>true, if <paramref cref="obj"/> is valid, false otherwise.</returns>
  bool IsValid(T obj);

}

/// <summary>
/// A set of extension methods for <see cref="IValidatable"/> objects
/// and their enumerations.
/// </summary>
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
