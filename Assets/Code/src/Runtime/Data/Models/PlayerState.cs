using HouraiTeahouse.Networking;
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A complete representation of a player's state at a given tick.
/// </summary>
[Serializable]
public struct PlayerState : INetworkSerializable {

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
  public bool IsTeetering;                            // One bit

  public uint JumpCount;                              // 1-4 bytes

  public uint RespawnTimeRemaining;                   // 1-4 bytes

  public uint StateID;                                // 1-4 bytes
  public uint StateTick;                              // 1-4 bytes

  public byte Charge;                                 // 1 byte

  public uint ShieldDamage;                           // 4 bytes
  public uint ShieldRecoveryCooldown;                 // 1-4 bytes

  public byte GrabbedLedgeID;                         // 1 byte

  // A timer for measuring the cooldown
  //
  // If greater than zero, the player is grabbing a ledge 
  // and it is a count of how many ticks remain before they automatically
  // let go.
  // 
  // If zero, the player is not grabbing a ledge.
  // 
  // If negative, the absolute value is the number of frames before
  // the player can grab a ledge again.
  public short GrabbedLedgeTimer;                     // 1-2 bytes

  public uint Hitstun;                                // 1-4 bytes
  public uint Hitlag;                                 // 1-4 bytes

  public sbyte Stocks;                                // 4 bytes

  // Bitfield: Each bit is whether the player has recently the player of that ID.
  // Each bit position corresponds to one player.
  public byte PlayersHit;                             // 1 byte

  public void GrabLedge(Ledge ledge) {
    GrabbedLedgeID = ledge.Id;
    GrabbedLedgeTimer = Config.Get<GameplayConfig>().MaxLedgeHangTime;
  }

  public void ReleaseLedge() {
    GrabbedLedgeID = 0;
    GrabbedLedgeTimer = (short)(-Config.Get<GameplayConfig>().LedgeGrabCooldown);
  }

  public void HitPlayer(int playerId) => PlayersHit |= (byte)(1 << playerId);
  public bool HasHit(int playerId) => (PlayersHit & (1 << playerId)) != 0;
  public void ResetPlayersHit() => PlayersHit = 0;

  void WriteBit(ref uint mask, bool val) {
    mask <<= 1;
    if (val) mask |= 1;
  }

  bool ReadBit(ref uint mask) {
    var result = (mask & 1) != 0;
    mask >>= 1;
    return result;
  }

  public void Serialize(ref Serializer writer) {
    uint mask = 0;
    WriteBit(ref mask, RespawnTimeRemaining != 0);
    WriteBit(ref mask, ShieldRecoveryCooldown != 0);
    WriteBit(ref mask, Hitstun != 0);
    WriteBit(ref mask, Hitlag != 0);
    WriteBit(ref mask, Charge != 0);
    WriteBit(ref mask, ShieldDamage != 0);
    WriteBit(ref mask, GrabbedLedgeTimer != 0);
    WriteBit(ref mask, GrabbedLedgeID != 0);
    WriteBit(ref mask, PlayersHit != 0);
    WriteBit(ref mask, JumpCount != 0);
    WriteBit(ref mask, Stocks != 0);
    WriteBit(ref mask, IsFastFalling);
    WriteBit(ref mask, Direction);

    writer.Write(mask);
    writer.Write(posX);
    writer.Write(posY);
    writer.Write(velX);
    writer.Write(velY);
    writer.Write(damage);
    writer.Write(StateID);
    writer.Write(StateTick);

    if (Stocks != 0)                 writer.Write(Stocks);
    if (JumpCount != 0)              writer.Write(JumpCount);
    if (PlayersHit != 0)             writer.Write(PlayersHit);
    if (GrabbedLedgeID != 0)         writer.Write(GrabbedLedgeID);
    if (GrabbedLedgeTimer != 0)      writer.Write(GrabbedLedgeTimer);
    if (ShieldDamage != 0)           writer.Write(ShieldDamage);
    if (Charge != 0)                 writer.Write(Charge);
    if (Hitlag != 0)                 writer.Write(Hitlag);
    if (Hitstun != 0)                writer.Write(Hitstun);
    if (ShieldRecoveryCooldown != 0) writer.Write(ShieldRecoveryCooldown);
    if (RespawnTimeRemaining != 0)   writer.Write(RespawnTimeRemaining);
  }

  public void Deserialize(ref Deserializer deserializer) {
    uint mask = deserializer.ReadUInt32();
    posX = deserializer.ReadInt16();
    posY = deserializer.ReadInt16();
    velX = deserializer.ReadInt32();
    velY = deserializer.ReadInt32();
    damage = deserializer.ReadUInt16();
    StateID = deserializer.ReadUInt32();
    StateTick = deserializer.ReadUInt32();

    Direction = ReadBit(ref mask);
    IsFastFalling = ReadBit(ref mask);
    if (ReadBit(ref mask)) Stocks = deserializer.ReadSByte();
    if (ReadBit(ref mask)) JumpCount = deserializer.ReadUInt32();
    if (ReadBit(ref mask)) PlayersHit = deserializer.ReadByte();
    if (ReadBit(ref mask)) GrabbedLedgeID = deserializer.ReadByte();
    if (ReadBit(ref mask)) GrabbedLedgeTimer = deserializer.ReadInt16();
    if (ReadBit(ref mask)) ShieldDamage = deserializer.ReadUInt32();
    if (ReadBit(ref mask)) Charge = deserializer.ReadByte();
    if (ReadBit(ref mask)) Hitlag = deserializer.ReadUInt32();
    if (ReadBit(ref mask)) Hitstun = deserializer.ReadUInt32();
    if (ReadBit(ref mask)) ShieldRecoveryCooldown = deserializer.ReadUInt32();
    if (ReadBit(ref mask)) RespawnTimeRemaining = deserializer.ReadUInt32();
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
    equals &= JumpCount == other.JumpCount;
    equals &= PlayersHit == other.PlayersHit;
    equals &= RespawnTimeRemaining == other.RespawnTimeRemaining;
    equals &= StateID == other.StateID;
    equals &= StateTick == other.StateTick;
    equals &= ShieldDamage == other.ShieldDamage;
    equals &= ShieldRecoveryCooldown == other.ShieldRecoveryCooldown;
    equals &= GrabbedLedgeID == other.GrabbedLedgeID;
    equals &= GrabbedLedgeTimer == other.GrabbedLedgeTimer;
    equals &= Damage == other.Damage;
    equals &= Hitstun == other.Hitstun;
    equals &= Charge == other.Charge;
    equals &= Stocks == other.Stocks;
    return equals;
  }

  public override int GetHashCode() {
    unchecked {
      int hash = 1367 * Position.GetHashCode();
      hash += 919 * Velocity.GetHashCode();
      hash += 373 * Direction.GetHashCode();
      hash += 199 * IsFastFalling.GetHashCode();
      hash += 131 * JumpCount.GetHashCode();
      hash += 101 * RespawnTimeRemaining.GetHashCode();
      hash += 83 * StateID.GetHashCode();
      hash += 71 * StateTick.GetHashCode();
      hash += 59 * ShieldDamage.GetHashCode();
      hash += 47 * ShieldRecoveryCooldown.GetHashCode();
      hash += 43 * GrabbedLedgeID;
      hash += 37 * GrabbedLedgeTimer;
      hash += 31 * Damage.GetHashCode();
      hash += 17 * Hitstun.GetHashCode();
      hash += 7 * PlayersHit.GetHashCode();
      hash += Stocks;
      return hash;
    }
  }
}

}