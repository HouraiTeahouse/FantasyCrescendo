using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(AudioSource))]
public class TimeScaledAudio : MonoBehaviour {

  public AudioSource AudioSource { get; private set; }

  float pitch_;

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