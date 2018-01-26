using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public static class UNETUtility {

  public static void HandleError(byte error) {
    if (error == 0) return;
    Debug.LogError($"Networking Error: {(NetworkError)error}");
  }

  public static Exception CreateError(byte error) {
    if (error == 0) return null;
    return new NetworkingException($"Networking Error: {(NetworkError)error}");
  }

}

}