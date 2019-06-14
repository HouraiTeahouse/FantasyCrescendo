using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HouraiTeahouse.FantasyCrescendo {

public class ErrorManager : MonoBehaviour {

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
    TriggerError(message);
  }

  public static void TriggerError(Exception exception) {
    TriggerError(exception?.Message ?? string.Empty);
  }

  public static async void TriggerError(string error) {
    await Addressables.LoadSceneAsync(Config.Get<SceneConfig>().ErrorScene);
    var errorScreen = FindObjectOfType<ErrorScreen>();
    if (errorScreen == null) return;
    errorScreen.SetError(error);
  }

}

}
