using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo {

public class VersionText : MonoBehaviour {

  public TMP_Text Text;
  public string Format;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (Debug.isDebugBuild) {
      if (string.IsNullOrEmpty(Format)) {
        Text.text = Application.version;
      } else {
        Text.text = string.Format(Format, Application.version);
      }
      Destroy(this);
    } else {
      Destroy(gameObject);
    }
  }

}

}