using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public static class UNETUtility {

  public static void HandleError(byte error) {
    if (error == 0) return;
    Debug.LogError($"Networking Error: {(NetworkError)error}");
  }

}

}