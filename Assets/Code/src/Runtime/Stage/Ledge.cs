using System.Collections;
using System.Collections.Generic;
using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(CapsuleCollider))]
public sealed class Ledge : RegisteredBehaviour<Ledge, byte> {

  public bool Direction;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  protected override void Awake() {
    base.Awake();
    foreach (var collider in GetComponents<Collider>()) {
      collider.isTrigger = true;
    }
  }

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    var capsuleCollider = GetComponent<CapsuleCollider>();
    if (capsuleCollider == null) return;
    using (GizmoUtil.With(Color.red)) {
      GizmoUtil.DrawCapsuleCollider(capsuleCollider);
    }
  }

  public bool IsOccupied(MatchState state) {
    bool occupied = false;
    for (uint i = 0; i < state.PlayerCount; i++) {
      occupied |= state.GetPlayerState(i).GrabbedLedgeID == Id;
    }
    return occupied;
  }

  /// <summary>
  /// Reset is called when the user hits the Reset button in the Inspector's
  /// context menu or when adding the component the first time.
  /// </summary>
  void Reset() {
    ResetID();
    // TODO(james7132): Add this to some config
    gameObject.tag = "Ledge";
  }

  [ContextMenu("Reset ID")]
  void ResetID() {
    var random = new Random();
    do {
      Id = (byte)random.Next();
    } while (Id == 0);
  }

}

}