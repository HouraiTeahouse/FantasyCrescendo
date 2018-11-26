using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterCamera : MonoBehaviour, IPlayerComponent {

  MatchCameraTarget _target;

  public Task Initialize(PlayerConfig config, bool isView = false) {
    _target = FindObjectOfType<MatchCameraTarget>();
    if (isView) {
      Register();
    }
    return Task.CompletedTask;
  }

  void OnEnable() => Register();

  void OnDisable() => Unregister();

  void Register() {
    if (_target != null) {
      _target.RegisterTarget(transform);
    }
  }

  void Unregister() {
    if (_target != null) {
      _target.UnregisterTarget(transform);
    }
  }

}

}
