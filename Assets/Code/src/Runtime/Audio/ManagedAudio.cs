using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary> A reference to an AudioSource managed by the AudioManager </summary>
[AddComponentMenu("")]
public class ManagedAudio : MonoBehaviour {

  /// <summary> The AudioSource for the object </summary>
  public AudioSource AudioSource { get; private set; }

  /// <summary> Awake is called when the script instance is being loaded. </summary>
  void Awake() {
    AudioSource = GetComponent<AudioSource>();
  }

  /// <summary> Update is called every frame, if the MonoBehaviour is enabled. </summary>
  void Update() {
    if (!AudioSource.isPlaying) {
      AudioManager.Instance.Return(this);
    }
  }

}

}