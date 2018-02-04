using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class VersionText : MonoBehaviour {

  public Text Text;
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