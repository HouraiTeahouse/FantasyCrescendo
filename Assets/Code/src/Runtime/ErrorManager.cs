using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class ErrorManager : MonoBehaviour {

  [RuntimeInitializeOnLoadMethod]
  static void OnAssemblyLoad() {
  }

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Application.logMessageReceived += OnLog;
  }

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() {
    Application.logMessageReceived -= OnLog;
  }

  void OnLog(string message, string stackTrace, LogType type) {
    if (type != LogType.Exception) return;
    TriggerError(message, false);
  }

  public static void TriggerError(Exception exception, bool isFatal = false) {
    TriggerError(exception?.Message ?? string.Empty);
  }

  public static async void TriggerError(string error, bool isFatal = false) {
    await Config.Get<SceneConfig>().ErrorScene.LoadAsync();
    var errorScreen = FindObjectOfType<ErrorScreen>();
    if (errorScreen == null) return;
    errorScreen.SetError(error, isFatal);
  }

}

}
