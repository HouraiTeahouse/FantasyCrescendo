using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class BlastZone : MonoBehaviour {

  public Bounds Bounds;

  public MatchState Simulate(MatchState state) {
    for (uint i = 0; i < state.PlayerCount; i++) {
      var playerState = state.GetPlayerState(i);
      if (Bounds.Contains(playerState.Position)) continue;
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