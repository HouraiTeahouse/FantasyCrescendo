using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public static class HitboxUtil {

  static Dictionary<HitboxType, Color> HitboxTypeColors;
  static Dictionary<HurtboxType, Color> HurtboxTypeColors;

  static Collider[] CollisionSet;
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

    const int kCacheSize = 256;
    CollisionSet = new Collider[kCacheSize];
    DedupCheck = new HashSet<Hurtbox>();
  }

  static bool IsValid(Hurtbox hurtbox) => hurtbox != null && DedupCheck.Add(hurtbox) && hurtbox.isActiveAndEnabled;

  static int CheckColliders(Hitbox hitbox) {
    var layerMask = Config.Get<PhysicsConfig>().HurtboxLayerMask;
    return Physics.OverlapSphereNonAlloc(hitbox.Center, hitbox.Radius, CollisionSet, layerMask, QueryTriggerInteraction.Collide);
  }

  public static int CollisionCheck(Hitbox hitbox, Hurtbox[] hurtboxSet) {
    var collisionCount = CheckColliders(hitbox);
    int hurtboxCount = 0;
    DedupCheck.Clear();
    for (int i = 0; i < collisionCount && hurtboxCount < hurtboxSet.Length; i++) {
      Assert.IsNotNull(CollisionSet[i]);
      var hurtbox = CollisionSet[i].GetComponent<Hurtbox>();
      if (IsValid(hurtbox)) {
        hurtboxSet[hurtboxCount++] = hurtbox;
      }
    }
    return hurtboxCount;
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
