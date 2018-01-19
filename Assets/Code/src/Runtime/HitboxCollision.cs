using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public struct HitboxCollision {
  public Hitbox Source;
  public Hurtbox Destination;

  public bool IsSelfCollision => Source.PlayerID == Destination.PlayerID;
  public uint Priority => Source.Priority * Destination.Priority;

  public override string ToString() => $"HitboxCollision ({Source} -> {Destination})";
}

}
