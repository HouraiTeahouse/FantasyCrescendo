using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PauseSFX : MonoBehaviour {

  public AudioSource AudioSource;
  public AudioClip PauseClip;
  public AudioClip UnpauseClip;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Mediator.Global.CreateUnityContext(this)
      .Subscribe<MatchPauseStateChangedEvent>(args => {
        if (AudioSource == null) return;
        var clip = args.Paused ? PauseClip : UnpauseClip;
        AudioSource.Stop();
        AudioSource.clip = clip;
        AudioSource.Play();
      });
  }

}

}