using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterCamera : MonoBehaviour, IPlayerComponent {

  public Task Initialize(PlayerConfig config, bool isView = false) {
    if (isView) {
      var matchCameraTarget = FindObjectOfType<MatchCameraTarget>();
      if (matchCameraTarget != null) {
        matchCameraTarget.RegisterTarget(transform);
      }
    }
    return Task.CompletedTask;
  }

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() {
    var matchCameraTarget = FindObjectOfType<MatchCameraTarget>();
    if (matchCameraTarget != null) {
      matchCameraTarget.UnregisterTarget(transform);
    }
  }

}

}
