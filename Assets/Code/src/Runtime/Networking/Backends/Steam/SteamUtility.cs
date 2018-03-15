using Steamworks;
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking.Steam {

public static class SteamUtility {

  /// <summary>
  /// Creates error logs for a given error code.
  /// Does nothing if there is no error.
  /// </summary>
  /// <param name="result">the error code of the error.</param>
  public static void HandleError(EResult result) {
    if (result == EResult.k_EResultOK) return;
    Debug.LogError($"Steam Networking Error: {result}");
  }

  public static bool IsError(EResult result) {
    return result != EResult.k_EResultOK;
  }

  /// <summary>
  /// Creates an exception out of a UNET NetworkError.
  /// </summary>
  /// <param name="result">the error code of the error.</param>
  /// <returns>an exception representing the error, null if no error.</returns>
  public static Exception CreateError(EResult result) {
    if (result == EResult.k_EResultOK) return null;
    return new NetworkingException($"Steam Networking Error: {result}");
  }

}

}