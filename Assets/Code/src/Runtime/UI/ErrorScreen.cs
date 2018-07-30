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

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() => SetFatal(false);

  public void SetError(string error, bool isFatal = false) {
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