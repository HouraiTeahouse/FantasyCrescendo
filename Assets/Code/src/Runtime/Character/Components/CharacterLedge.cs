using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterLedge : MonoBehaviour, IPlayerSimulation, IPlayerView {

  public Bounds[] CheckRegions;

  bool isView; 
  bool dir;

  public Task Initialize(PlayerConfig config, bool isView) {
    this.isView = isView;
    return Task.CompletedTask;
  }

  public void Presimulate(ref PlayerState state) => ApplyState(ref state);

  public void Simulate(ref PlayerState state, PlayerInputContext input) {
    dir = state.Direction;
    if (!state.IsGrabbingLedge && state.GrabbedLedgeTimer >= 0) {
      var ledge = LedgeUtil.CheckForLedges(state, CheckRegions, transform.position);
      if (ledge?.IsOccupied(state.MatchState) == false) {
        state.GrabLedge(ledge);
      }
    }
  }

  public void ApplyState(ref PlayerState state) => dir = state.Direction;

  public void ResetState(ref PlayerState state) {
    state.GrabbedLedgeID = 0;
    state.GrabbedLedgeTimer = 0;
  }

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    if (CheckRegions == null) return;
#if UNITY_EDITOR
    if (EditorApplication.isPlayingOrWillChangePlaymode && !isView) return;
#endif
    using (GizmoUtil.With(Color.white))  {
      foreach (var region in CheckRegions) {
        GizmoUtil.DrawBox(LedgeUtil.GetWorldRegion(region, transform.position, dir));
      }
    }
  }

}

}