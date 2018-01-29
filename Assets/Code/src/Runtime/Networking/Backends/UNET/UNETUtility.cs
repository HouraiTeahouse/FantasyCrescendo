using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

/// <summary>
/// Utility class for handling UNET message results.
/// </summary>
public static class UNETUtility {

  /// <summary>
  /// Creates error logs for a given error code.
  /// Does nothing if there is no error.
  /// </summary>
  /// <param name="error">the error code of the error.</param>
  public static void HandleError(byte error) {
    if (error == 0) return;
    Debug.LogError($"Networking Error: {(NetworkError)error}");
  }

  /// <summary>
  /// Creates an exception out of a UNET NetworkError.
  /// </summary>
  /// <param name="error">the error code of the error.</param>
  /// <returns>an exception representing the error, null if no error.</returns>
  public static Exception CreateError(byte error) {
    if (error == 0) return null;
    return new NetworkingException($"Networking Error: {(NetworkError)error}");
  }

}

}