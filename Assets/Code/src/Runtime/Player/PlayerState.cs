using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Networking;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Players {

/// <summary>
/// A complete representation of a player's state at a given tick.
/// </summary>
[Serializable]
public struct PlayerState {

  [NonSerialized] public MatchState MatchState;
  public bool IsActive => Stocks > 0;
  public bool IsGrabbingLedge => GrabbedLedgeID != 0;
  public bool IsRespawning => RespawnTimeRemaining > 0;
  public bool IsHit => Hitstun > 0;
  public float StateTime => StateTick * Time.fixedDeltaTime;

  ushort damage;                                      // 2 bytes
  public float Damage {
    get { return damage / (float)(1 << 6); }
    set { damage = (ushort)(value * (1 << 6)); }
  }

  short posX, posY;                                   // 4 bytes
  public float PositionX {
    get { return posX / (float)(1 << 8); }
    set { posX = (short)(value * (1 << 8)); }
  }
  public float PositionY {
    get { return posY / (float)(1 << 8); }
    set { posY = (short)(value * (1 << 8)); }
  }
  public Vector2 Position {
    get { return new Vector2(PositionX, PositionY); }
    set { PositionX = value.x; PositionY = value.y; }
  }

  int velX, velY;                                     // 2-6 bytes
  public float VelocityX {
    get { return velX / (float)(1 << 8); }
    set { velX = (short)(value * (1 << 8)); }
  }
  public float VelocityY {
    get { return velY / (float)(1 << 8); }
    set { velY = (int)(value * (1 << 8)); }
  }
  public Vector2 Velocity {
    get { return new Vector2(VelocityX, VelocityY); }
    set { VelocityX = value.x; VelocityY = value.y; }
  }

  // Direction: True => Right, False => Left
  public bool Direction;                              // One bit
  public bool IsFastFalling;                          // One bit

  public uint RemainingJumps;                         // 1-4 bytes

  public uint RespawnTimeRemaining;                   // 1-4 bytes

  public uint StateID;                                // 1-4 bytes
  public uint StateTick;                              // 1-4 bytes

  public uint ShieldDamage;                           // 4 bytes
  public uint ShieldRecoveryCooldown;                 // 1-4 bytes

  public byte GrabbedLedgeID;                         // 1 byte

  public uint Hitstun;                                // 1-4 bytes

  public sbyte Stocks;                                // 4 bytes

  void WriteBit(ref uint mask, bool val) {
    mask <<= 1;
    if (val) mask |= 1;
  }

  bool ReadBit(ref uint mask) {
    var result = (mask & 1) != 0;
    mask >>= 1;
    return result;
  }

  public void Serialize(NetworkWriter writer) {
    uint mask = 0;
    WriteBit(ref mask, RespawnTimeRemaining != 0);
    WriteBit(ref mask, ShieldRecoveryCooldown != 0);
    WriteBit(ref mask, Hitstun != 0);
    WriteBit(ref mask, ShieldDamage != 0);
    WriteBit(ref mask, GrabbedLedgeID != 0);
    WriteBit(ref mask, RemainingJumps != 0);
    WriteBit(ref mask, Stocks != 0);
    WriteBit(ref mask, IsFastFalling);
    WriteBit(ref mask, Direction);

    writer.WritePackedUInt32(mask);
    writer.Write(posX);
    writer.Write(posY);
    writer.WritePackedInt32(velX);
    writer.WritePackedInt32(velY);
    writer.Write(damage);
    writer.WritePackedUInt32(StateID);
    writer.WritePackedUInt32(StateTick);

    if (Stocks != 0)                 writer.Write(Stocks);
    if (RemainingJumps != 0)         writer.WritePackedUInt32(RemainingJumps);
    if (GrabbedLedgeID != 0)         writer.Write(GrabbedLedgeID);
    if (ShieldDamage != 0)           writer.WritePackedUInt32(ShieldDamage);
    if (Hitstun != 0)                writer.WritePackedUInt32(Hitstun);
    if (ShieldRecoveryCooldown != 0) writer.WritePackedUInt32(ShieldRecoveryCooldown);
    if (RespawnTimeRemaining != 0)   writer.WritePackedUInt32(RespawnTimeRemaining);
  }

  public void Deserialize(NetworkReader reader) {
    uint mask = reader.ReadPackedUInt32();
    posX = reader.ReadInt16();
    posY = reader.ReadInt16();
    velX = reader.ReadPackedInt32();
    velY = reader.ReadPackedInt32();
    damage = reader.ReadUInt16();
    StateID = reader.ReadPackedUInt32();
    StateTick = reader.ReadPackedUInt32();

    Direction = ReadBit(ref mask);
    IsFastFalling = ReadBit(ref mask);
    if (ReadBit(ref mask)) Stocks = reader.ReadSByte();
    if (ReadBit(ref mask)) RemainingJumps = reader.ReadPackedUInt32();
    if (ReadBit(ref mask)) GrabbedLedgeID = reader.ReadByte();
    if (ReadBit(ref mask)) ShieldDamage = reader.ReadPackedUInt32();
    if (ReadBit(ref mask)) Hitstun = reader.ReadPackedUInt32();
    if (ReadBit(ref mask)) ShieldRecoveryCooldown = reader.ReadPackedUInt32();
    if (ReadBit(ref mask)) RespawnTimeRemaining = reader.ReadPackedUInt32();
  }

  // TODO(james7132): See if there's a better way to do this
  public override bool Equals(object obj) {
    if (!(obj is PlayerState)) return false;
    var other = (PlayerState)obj;
    if (!IsActive && !other.IsActive) return true;
    bool equals = Position == other.Position;
    equals &= Velocity == other.Velocity;
    equals &= Direction == other.Direction;
    equals &= IsFastFalling == other.IsFastFalling;
    equals &= RemainingJumps == other.RemainingJumps;
    equals &= RespawnTimeRemaining == other.RespawnTimeRemaining;
    equals &= StateID == other.StateID;
    equals &= StateTick == other.StateTick;
    equals &= ShieldDamage == other.ShieldDamage;
    equals &= ShieldRecoveryCooldown == other.ShieldRecoveryCooldown;
    equals &= GrabbedLedgeID == other.GrabbedLedgeID;
    equals &= Damage == other.Damage;
    equals &= Hitstun == other.Hitstun;
    equals &= Stocks == other.Stocks;
    return equals;
  }

  public override int GetHashCode() {
    unchecked {
      int hash = 1367 * Position.GetHashCode();
      hash += 919 * Velocity.GetHashCode();
      hash += 373 * Direction.GetHashCode();
      hash += 199 * IsFastFalling.GetHashCode();
      hash += 131 * RemainingJumps.GetHashCode();
      hash += 101 * RespawnTimeRemaining.GetHashCode();
      hash += 83 * StateID.GetHashCode();
      hash += 71 * StateTick.GetHashCode();
      hash += 59 * ShieldDamage.GetHashCode();
      hash += 47 * ShieldRecoveryCooldown.GetHashCode();
      hash += 43 * GrabbedLedgeID;
      hash += 31 * Damage.GetHashCode();
      hash += 17 * Hitstun.GetHashCode();
      hash += Stocks;
      return hash;
    }
  }
}

}