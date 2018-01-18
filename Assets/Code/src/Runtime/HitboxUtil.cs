using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public static class HitboxUtil {

  static Dictionary<HitboxType, Color> HitboxTypeColors;
  static Dictionary<HurtboxType, Color> HurtboxTypeColors;

  static HitboxUtil() {
    HitboxTypeColors = new Dictionary<HitboxType, Color> {
      { HitboxType.Offensive, Color.red },
    };
    HurtboxTypeColors = new Dictionary<HurtboxType, Color> {
      { HurtboxType.Damageable, Color.yellow },
      { HurtboxType.Invincible, Color.green },
      { HurtboxType.Intangible, Color.blue },
      { HurtboxType.Shield, Color.cyan }
    };
  }

  public static Color GetHitboxColor(HitboxType type) {
    Color typeColor;
    if (HitboxTypeColors.TryGetValue(type, out typeColor)) {
      return typeColor;
    }
    return Color.grey;
  }

  public static Color GetHurtboxColor(HurtboxType type) {
    Color typeColor;
    if (HurtboxTypeColors.TryGetValue(type, out typeColor)) {
      return typeColor;
    }
    return Color.grey;
  }

}

}
