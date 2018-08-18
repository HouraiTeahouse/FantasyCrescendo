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

public class CharacterTeeter : MonoBehaviour, IPlayerSimulation, IPlayerView {

  public Bounds[] CheckRegions;

  bool isView; 
  bool dir;

  public Task Initialize(PlayerConfig config, bool isView) {
    this.isView = isView;
    return Task.CompletedTask;
  }

  public void Presimulate(ref PlayerState state) => ApplyState(ref state);

  // TODO: Fix bug where pressing down+right/down+left will switch your direction but you do not move
  // As such, you continue to teeter even in the wrong direction
  public void Simulate(ref PlayerState state, PlayerInputContext input) {
    dir = state.Direction;
    if (state.IsTeetering) {
      if (state.Velocity != Vector2.zero) {
        state.IsTeetering = false;
      }
    } else {
      if (state.Velocity == Vector2.zero) {
        var ledge = LedgeUtil.CheckForLedges(state, CheckRegions, transform.position);
        if (ledge != null){
          state.IsTeetering = ledge.Direction ^ dir;
        }
      }
    }
  }

  public void ApplyState(ref PlayerState state) => dir = state.Direction;

  public void ResetState(ref PlayerState state) {
    state.IsTeetering = false;
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