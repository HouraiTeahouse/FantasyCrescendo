using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct HitboxEntry {

  public Vector3 Position { get; set; }
  public float Radius { get; set; }

  public int OwnerID { get; set; }

  public HitboxType Type;
  public int Priority;

  public float Damage;
  public float BaseKnockback;
  public float KnockbackScaling;

  public bool CollidesWith(HitboxEntry entry) {
    var centerSquareDistance = (entry.Position - Position).sqrMagnitude;
    var combinedRadii = entry.Radius + Radius;
    return centerSquareDistance < combinedRadii * combinedRadii;
  }

  public float GetKnockback(float damage) {
    return BaseKnockback + damage * KnockbackScaling;
  }

}

}
