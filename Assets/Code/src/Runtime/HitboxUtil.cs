using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public static class HitboxUtil {

  static Dictionary<HitboxType, Color> HitboxTypeColors;
  static Dictionary<HurtboxType, Color> HurtboxTypeColors;

  static HashSet<Hurtbox> DedupCheck;

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

    DedupCheck = new HashSet<Hurtbox>();
  }

  public static void FlushHurtboxDedupCache() => DedupCheck.Clear();

  public static bool IsValidHurtbox(Hurtbox hurtbox) => hurtbox != null && DedupCheck.Add(hurtbox) && hurtbox.isActiveAndEnabled;

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
