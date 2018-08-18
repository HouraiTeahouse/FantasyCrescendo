using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public static class LedgeUtil {

  public static Ledge CheckForLedges(PlayerState state, Bounds[] regions, Vector3 position) {
    Ledge ledge = null;
    var arrayPool = ArrayPool<Collider>.Shared;
    var layerMask = Config.Get<PhysicsConfig>().StageLayers;
    var colliders = arrayPool.Rent(256);
    foreach (var region in regions) {
      var worldRegion = GetWorldRegion(region, position, state.Direction);
      var colliderCount = Physics.OverlapBoxNonAlloc(worldRegion.center, worldRegion.extents, colliders, Quaternion.identity, layerMask, QueryTriggerInteraction.Collide);
      for (var i = 0; i < colliderCount; i++) {
        ledge = colliders[i].GetComponent<Ledge>();
        if (ledge != null) break;
      }
      if (ledge != null) break;
    }
    arrayPool.Return(colliders);
    return ledge;
  }

  public static Bounds GetWorldRegion(Bounds region, Vector3 position, bool direction){
    var center = region.center;
    if (direction) {
      center.x *= -1;
    }
    region.center = center + position;
    return region;
  }

}
}
