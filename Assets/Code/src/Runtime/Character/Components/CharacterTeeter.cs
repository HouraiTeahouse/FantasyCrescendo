﻿using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterTeeter : PlayerComponent {

  public Bounds[] CheckRegions;

  bool isView; 
  bool dir;

  public override Task Initialize(PlayerConfig config, bool isView) {
    this.isView = isView;
    return Task.CompletedTask;
  }

  public override void Simulate(ref PlayerState state, in PlayerInputContext input) {
    if (state.IsTeetering) {
      if (state.Velocity != Vector2.zero) {
        state.IsTeetering = false;
      }
    } else {
      if (state.Velocity == Vector2.zero) {
        state.IsTeetering = CheckForTeeter(state);
      }
    }
  }

  public override void UpdateView(in PlayerState state) => dir = state.Direction;

  public override void ResetState(ref PlayerState state) {
    state.IsTeetering = false;
  }

  bool CheckForTeeter(PlayerState state) {
    dir = state.Direction;
    Ledge ledge = null;
    var arrayPool = ArrayPool<Collider>.Shared;
    var layerMask = Config.Get<PhysicsConfig>().StageLayers;
    var colliders = arrayPool.Rent(256);
    foreach (var region in CheckRegions) {
      var worldRegion = GetWorldRegion(region, state.Direction);
      var colliderCount = Physics.OverlapBoxNonAlloc(worldRegion.center, worldRegion.extents, colliders, Quaternion.identity, layerMask, QueryTriggerInteraction.Collide);
      for (var i = 0; i < colliderCount; i++) {
        ledge = colliders[i].GetComponent<Ledge>();
        if (ledge != null) break;
      }
      if (ledge != null) break;
    }
    arrayPool.Return(colliders);
    
    if (ledge == null) {
      return false;
    } else {
      return dir ^ ledge.Direction;
    }
  }

  Bounds GetWorldRegion(Bounds region, bool direction) {
    var center = region.center;
    if (direction) {
      center.x *= -1;
    }
    region.center = center + transform.position;
    return region;
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
        GizmoUtil.DrawBox(GetWorldRegion(region, dir));
      }
    }
  }

}

}