using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public static class LedgeUtil {

  /// <summary>
  /// Returns the first ledge found within the regions and position.
  /// Returns null if no ledge was found
  /// </summary>
  /// <param name="state"></param>
  /// <param name="regions"></param>
  /// <param name="character position"></param>
  /// <returns>First Ledge found within bounds</returns>
  public static Ledge CheckForLedges(PlayerState state, Bounds[] regions, Vector3 position){
    return CheckForLedges(state, regions, position, false);
  }

  /// <summary>
  /// Returns the first ledge found within the regions and position.
  /// Also checks if the ledge is the same direction as the provided direction.
  /// Returns null if no ledge was found
  /// </summary>
  /// <param name="state"></param>
  /// <param name="regions"></param>
  /// <param name="position"></param>
  /// <param name="direction"></param>
  /// <returns>First Ledge found within bounds and provided direction.</returns>
  public static Ledge CheckForLedges(PlayerState state, Bounds[] regions, Vector3 position, bool direction){
    return CheckForLedges(state, regions, position, true, direction);
  }

  private static Ledge CheckForLedges(PlayerState state, Bounds[] regions, Vector3 position, bool forcedDirection, bool direction = false) {
    Ledge ledge = null;
    var arrayPool = ArrayPool<Collider>.Shared;
    var layerMask = Config.Get<PhysicsConfig>().StageLayers;
    var colliders = arrayPool.Rent(256);
    foreach (var region in regions) {
      var worldRegion = GetWorldRegion(region, position, state.Direction);
      var colliderCount = Physics.OverlapBoxNonAlloc(worldRegion.center, worldRegion.extents, colliders, Quaternion.identity, layerMask, QueryTriggerInteraction.Collide);
      for (var i = 0; i < colliderCount; i++) {
        var temp = colliders[i].GetComponent<Ledge>();
        if (temp == null) continue;
        if (forcedDirection && !(temp.Direction ^ direction)) continue;
        ledge = temp;
        break;
      }
      if (ledge != null) break;
    }
    arrayPool.Return(colliders);
    return ledge;
  }

  /// <summary>
  /// Converts local Ledge region to world region.
  /// </summary>
  /// <param name="region"></param>
  /// <param name="position"></param>
  /// <param name="direction"></param>
  /// <returns>Translated world region</returns>
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
