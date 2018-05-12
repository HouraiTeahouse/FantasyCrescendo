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

  static bool IsValid(Hurtbox hurtbox) => hurtbox != null && DedupCheck.Add(hurtbox) && hurtbox.isActiveAndEnabled;

  static int CheckColliders(Hitbox hitbox, Collider[] colliders) {
    var layerMask = Config.Get<PhysicsConfig>().HurtboxLayerMask;
    return Physics.OverlapSphereNonAlloc(hitbox.Center, hitbox.Radius, colliders, layerMask, QueryTriggerInteraction.Collide);
  }

  public static int CollisionCheck(Hitbox hitbox, Hurtbox[] hurtboxSet) {
    var colliders = ArrayPool<Collider>.Shared.Rent(256);
    var collisionCount = CheckColliders(hitbox, colliders);
    int hurtboxCount = 0;
    DedupCheck.Clear();
    for (int i = 0; i < collisionCount && hurtboxCount < hurtboxSet.Length; i++) {
      Assert.IsNotNull(colliders[i]);
      var hurtbox = colliders[i].GetComponent<Hurtbox>();
      if (IsValid(hurtbox)) {
        hurtboxSet[hurtboxCount++] = hurtbox;
      }
    }
    ArrayPool<Collider>.Shared.Return(colliders);
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
