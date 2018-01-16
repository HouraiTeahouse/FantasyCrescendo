using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PauseVisibility : MonoBehaviour {

  public Object[] Objects;
  public bool Invert;
  public bool DefaultState;
  
  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Mediator.Global.CreateUnityContext(this)
      .Subscribe<MatchPauseStateChangedEvent>(args => {
        SetActive(Invert ? !args.Paused : args.Paused);
      });
    SetActive(DefaultState);
  }

  void SetActive(bool active) {
    foreach (var obj in Objects) {
      ObjectUtil.SetActive(obj, active);
    }
  }

}

}