using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class RequireSteam : MonoBehaviour {

  public bool Invert;
  public Object[] toEnable;
  public Object[] toDisable;

  void OnEnable() {
    var hasSteam = SteamManager.Initialized;
    if (Invert) {
      hasSteam = !hasSteam;
    }
    foreach (var target in toEnable) {
      ObjectUtil.SetActive(target, hasSteam);
    }
    foreach (var target in toDisable) {
      ObjectUtil.SetActive(target, !hasSteam);
    }
  }

}

}