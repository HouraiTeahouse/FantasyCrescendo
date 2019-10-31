using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class PauseSFX : EventHandlerBehaviour<MatchPauseStateChangedEvent> {

  public AudioSource AudioSource;
  public AudioClip PauseClip;
  public AudioClip UnpauseClip;

  protected override void OnEvent(MatchPauseStateChangedEvent evt) {
    if (AudioSource == null) return;
    var clip = evt.IsPaused ? PauseClip : UnpauseClip;
    AudioSource.Stop();
    AudioSource.clip = clip;
    AudioSource.Play();
  }

}

}
