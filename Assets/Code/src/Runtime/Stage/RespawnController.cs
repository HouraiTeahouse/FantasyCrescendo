using HouraiTeahouse.FantasyCrescendo.Players;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Stages {

public class RespawnController : MonoBehaviour {

  public Transform[] RespawnPositions;
  public Vector3 RespawnBounds = Vector3.one / 2;
  public float RespawnTime = 3f;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Mediator.Global.CreateUnityContext(this).Subscribe<PlayerRespawnEvent>(OnPlayerDied);
  }

  void OnPlayerDied(PlayerRespawnEvent evt) {
    var playerState = evt.PlayerState;
    playerState.RespawnTimeRemaining = (uint)Mathf.FloorToInt(RespawnTime / Time.fixedDeltaTime);
    var state = evt.MatchState;
    foreach (var respawnPosition in RespawnPositions) {
      if (respawnPosition == null) continue;
      var bounds = new Bounds(respawnPosition.position, RespawnBounds);
      bool occupied = false;
      for (uint i = 0; i < state.PlayerCount; i++) {
        occupied |= bounds.Contains(state.GetPlayerState(i).Position);
      }
      if (occupied) continue;
      playerState.Position = respawnPosition.position;
      break;
    }
    evt.PlayerState = playerState;
  }

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    if (RespawnPositions == null) return;
    foreach (var respawnPosition in RespawnPositions) {
      if (respawnPosition == null) continue;
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(respawnPosition.position, 0.1f);
      Gizmos.color = Color.grey;
      Gizmos.DrawWireCube(respawnPosition.position, RespawnBounds);
    }
  }

}

}