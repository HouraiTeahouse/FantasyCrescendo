using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class ErrorScreen : MonoBehaviour {

  public Text[] ErrorText;
  public string Format;

  public GameObject FatalView;
  public GameObject NormalView;

  [RuntimeInitializeOnLoadMethod]
  static void OnAssemblyLoad() {
    Application.logMessageReceived += (m, _, type) => {
      if (type != LogType.Exception) return;
      TriggerError(m, false);
    };
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

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() => SetFatal(false);

  void SetError(string error, bool isFatal = false) {
    if (ErrorText == null || ErrorText.Length <= 0) return;
    if (string.IsNullOrEmpty(error)) {
      SetText(string.Empty);
    } else if (string.IsNullOrEmpty(Format)) {
      SetText(error);
    } else {
      SetText(string.Format(Format, error));
    }
    SetFatal(isFatal);
  }

  void SetFatal(bool isFatal) {
    ObjectUtil.SetActive(FatalView, isFatal);
    ObjectUtil.SetActive(NormalView, !isFatal);
  }

  void SetText(string text) {
    foreach (var errorText in ErrorText) {
      if (errorText == null) continue;
      errorText.text = text;
    }
  }

  public void OpenURL(string url) => Application.OpenURL(url);

  public void QuitGame() => Application.Quit();

}

}