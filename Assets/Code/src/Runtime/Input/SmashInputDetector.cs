using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class SmashInputDetector {

  const float kDampeningFactor = 0.85f;

  Vector2[] pastQueue;
  int index;

  public SmashInputDetector(int tickHistorySize) {
    pastQueue = new Vector2[tickHistorySize];
    index = 0;
  }

  public void Update(Vector2 movement) {
    pastQueue[index] = movement;
    index = (index + 1) % pastQueue.Length;
  }

  public Vector2 GetSmash(Vector2 movement) {
    var lastInput = Vector2.zero;
    var lastVel = Vector2.zero;

    var acc = Vector2.zero;

    var dt = Time.fixedDeltaTime;
    for (var i = index + 1; i < pastQueue.Length; i++) {
      var inputVel = (pastQueue[i] - lastInput) / dt;
      var inputAccel = (inputVel - lastVel) / dt;
      lastInput = pastQueue[i];
      lastVel = inputVel;
      if (DirectionalInput.GetDirection(movement) !=
          DirectionalInput.GetDirection(inputAccel) ||
          inputAccel.sqrMagnitude < acc.sqrMagnitude) {
        continue;
      }
      acc = inputAccel;
    }
    for (var i = 0; i <= index; i++) {
      var inputVel = (pastQueue[i] - lastInput) / dt;
      var inputAccel = (inputVel - lastVel) / dt;
      lastInput = pastQueue[i];
      lastVel = inputVel;
      if (DirectionalInput.GetDirection(movement) !=
          DirectionalInput.GetDirection(acc) ||
          inputAccel.sqrMagnitude < acc.sqrMagnitude) {
        continue;
      }
      acc = inputAccel;
    }
    return acc;
  }

}

}