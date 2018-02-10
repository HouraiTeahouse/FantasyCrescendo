using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A script for setting up a UI Text object with text from a TextAsset.
/// </summary>
public class TestAssetDisplay : MonoBehaviour {

  public Text Text;
  public TextAsset TextAsset;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() => SetText(TextAsset);

  /// <summary>
  /// Sets the text of the UI Text to the string data stored in a
  /// provided TextAsset.
  /// </summary>
  /// <param name="asset">the source TextAsset.</param>
  public void SetText(TextAsset asset) {
    if (asset == null || Text == null) return;
    TextAsset = asset;
    Text.text = asset.text;
  }

}

}