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

}
