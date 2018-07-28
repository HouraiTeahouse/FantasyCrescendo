using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// Data object for playing sound effects and displaying game state.
/// Controls an AudioSource and Text display object.
/// </summary>
[System.Serializable]
public struct MatchStateDisplayData {
  public AudioClip Clip;
  public string Text;
  public TMP_ColorGradient Color;

  /// <summary>
  /// Plays <see cref="Clip"/> on the provided <paramref cref="audioSource"/> and
  /// sets the provided <paramref cref="text"/> to show <see cref="Text"/> with the 
  /// gradient described by <see cref="Color"/>.
  /// </summary>
  /// <remarks>
  /// Will not do anything if the provided components are null.
  /// </remarks>
  /// <param name="audioSource">the AudioSource to play the clip.</param>
  /// <param name="text">the text UI object to project onto.</param>
  public void Apply(AudioSource audioSource, TMP_Text text) {
    if (text != null) {
      text.text = Text;
      text.colorGradientPreset = Color;
      text.enableVertexGradient = Color != null;
    }
    if (audioSource != null) {
      audioSource.clip = Clip;
      audioSource.Play();
    }
  }
}

}
