using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo {

public class ErrorScreen : MonoBehaviour {

  public TMP_Text ErrorText;
  public string Format;

  public void SetError(string error) {
    if (ErrorText == null) return;
    if (string.IsNullOrEmpty(error)) {
      ErrorText.text = string.Empty;
    } else if (string.IsNullOrEmpty(Format)) {
      ErrorText.text = error;
    } else {
      ErrorText.text = string.Format(Format, error);
    }
  }

  public void OpenURL(string url) => Application.OpenURL(url);

  public void QuitGame() => Application.Quit();

}

}