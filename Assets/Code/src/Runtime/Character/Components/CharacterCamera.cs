using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterCamera : MonoBehaviour, IPlayerComponent {

  static MatchCameraTarget _target;
  static MatchCameraTarget Target => _target ?? (_target = FindObjectOfType<MatchCameraTarget>());

  public Task Initialize(PlayerConfig config, bool isView = false) {
    if (isView && Target != null ) {
      _target.RegisterTarget(transform);
    }
    return Task.CompletedTask;
  }

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() {
    if (Target != null) {
      _target.UnregisterTarget(transform);
    }
  }

}

}
