using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public static class HitboxUtil {

  static Dictionary<HitboxType, Color> TypeColors;

  static HitboxUtil() {
    TypeColors = new Dictionary<HitboxType, Color> {
      { HitboxType.Offensive, Color.red },
      { HitboxType.Damageable, Color.yellow },
      { HitboxType.Invincible, Color.green },
      { HitboxType.Intangible, Color.blue }
    };
  }

  public static Color GetHitboxColor(HitboxType type) {
    Color typeColor;
    if (TypeColors.TryGetValue(type, out typeColor)) {
      return typeColor;
    }
    return Color.grey;
  }

  public static IEnumerable<HitboxCollision> CreateCollisions(
      IEnumerable<HitboxEntry> source,
      IEnumerable<HitboxEntry> dest) {
    foreach (var sourceEntry in source) {
      foreach (var destEntry in dest) {
        if (sourceEntry.CollidesWith(destEntry)){
          yield return new HitboxCollision {
            Source = sourceEntry,
            Destination = destEntry
          };
        }
      }
    }
  }

}

}
