using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Stages {

public class BlastZone : MonoBehaviour {

  public Bounds Bounds;

  public MatchState Simulate(MatchState state) {
    for (var i = 0; i < state.PlayerCount; i++) {
      if (Bounds.Contains(state[i].Position)) continue;
      Mediator.Global.Publish(new PlayerDiedEvent {
        PlayerID = i,
        MatchState = state
      });
    }
    return state;
  }

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireCube(Bounds.center, Bounds.size);
  }

}

}