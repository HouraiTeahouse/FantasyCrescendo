using System; 

namespace HouraiTeahouse.FantasyCrescendo {

public struct HitboxCollision : IComparable<HitboxCollision> {
  public Hitbox Source;
  public Hurtbox Destination;

  public bool IsSelfCollision => Source.PlayerID == Destination.PlayerID;
  public uint Priority => Source.Priority * Destination.Priority;

  public int CompareTo(HitboxCollision obj) {
    int typeCompare = Destination.Type.CompareTo(obj.Destination.Type);
    if (typeCompare != 0)
      return typeCompare;
    return Priority.CompareTo(obj.Priority);
  }

  public override string ToString() => $"HitboxCollision ({Source} -> {Destination})";

  public void PlayEffect(HitInfo hitInfo) => Source.PlayEffect(hitInfo);

}

}