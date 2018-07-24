using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary> 
/// A MonoBehaivour that will stretch out the length and pitch of an AudioSource based on Time.deltaTime 
/// </summary>
/// <remarks>
/// This has the effect of stopping audio while Time.timeScale is 0 (game is paused). 
/// As well as slowing down/speeding up sounds when the game is slowed or sped up.
/// 
/// This is done by changing the Pitch of the AudioSource this component controls.
/// It is better to use the <see cref="Pitch"/> property on the component over directly
/// alterting the pitch of the AudioSource.
/// </remarks>
[RequireComponent(typeof(AudioSource))]
public class TimeScaledAudio : MonoBehaviour {

  /// <summary>
  /// The controlled AudioSource.
  /// </summary>
  public AudioSource AudioSource { get; private set; }

  float pitch_;

  /// <summary>
  /// Gets or sets original pitch of AudioSource.
  /// </summary>
  public float Pitch {
    get { return pitch_; }
    set { 
      pitch_ = value;
      AudioSource.pitch = pitch_ * Time.timeScale;
    }
  }

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    AudioSource = GetComponent<AudioSource>();
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    AudioSource.pitch = pitch_ * Time.timeScale;
  }

}

}