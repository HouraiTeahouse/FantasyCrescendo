using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class PauseVisibility : EventHandlerBehaviour<MatchPauseStateChangedEvent> {

  public Object[] Objects;
  public bool Invert;
  public bool DefaultState;
  
  protected override void Awake() {
    base.Awake();
    SetActive(DefaultState);
  }

  protected override void OnEvent(MatchPauseStateChangedEvent evt) =>
    SetActive(Invert ? !evt.IsPaused : evt.IsPaused);

  void SetActive(bool active) {
    foreach (var obj in Objects) {
      ObjectUtility.SetActive(obj, active);
    }
  }

}

}