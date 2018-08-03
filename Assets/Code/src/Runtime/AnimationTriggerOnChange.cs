using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class AnimationTriggerOnChange : MonoBehaviour {
  public Animator[] Animators;
  public string Trigger;
  public bool TriggerOnEnable;
  public bool TriggerOnDisable;

  int _triggerHash;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    _triggerHash = Animator.StringToHash(Trigger);
  }

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => SetTrigger(TriggerOnEnable);

  /// <summary>
  /// This function is called when the behaviour becomes disabled or inactive.
  /// </summary>
  void OnDisable() => SetTrigger(TriggerOnDisable);

  void SetTrigger(bool shouldTrigger) {
    if (!shouldTrigger) return;
    foreach (var animator in Animators) {
      if (animator == null) continue;
      animator.SetTrigger(_triggerHash);
    }
  }

}

}