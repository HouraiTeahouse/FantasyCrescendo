using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A complete representation of a player's state at a given tick.
/// </summary>
[Serializable]
public struct PlayerState {

  // One Player Total: 18-53 bytes
  // 4 Player: 72-176 bytes
  //
  // Send Rate | Minimum | Maximum
  // -------------------------------
  // 15/s (x1) | 270     | 795
  // 15/s (x4) | 4320    | 10560
  // 30/s (x1) | 540     | 1590
  // 30/s (x4) | 8640    | 21120
  // 60/s (x1) | 1080    | 3180
  // 60/s (x4) | 17280   | 42240

  // TODO(james7132): Generalize this
  [NonSerialized] public MatchState MatchState;
  public bool IsActive => Stocks > 0;
  public bool IsGrabbingLedge => GrabbedLedgeID != 0;
  public bool IsRespawning => RespawnTimeRemaining > 0;
  public bool IsHit => Hitstun > 0;

  public Vector2 Position;                            // 8 bytes
  public Vector2 Velocity;                            // 8 bytes

  // Direction: True => Right, False => Left
  public bool Direction;                              // One bit
  public bool IsFastFalling;                          // One bit

  public uint RemainingJumps;                         // 1-4 bytes

  public uint RespawnTimeRemaining;                   // 1-4 bytes

  public int StateHash;                               // 1-4 bytes
  public float NormalizedStateTime;                   // 4 bytes

  public uint ShieldDamage;                          // 4 bytes
  public uint ShieldRecoveryCooldown;                 // 1-4 bytes

  public byte GrabbedLedgeID;                         // 1 byte

  public float Damage;                                // 4 bytes
  public uint Hitstun;                                // 1-4 bytes

  public int Stocks;                                  // 4 bytes

  // TODO(james7132): See if there's a better 
  public override bool Equals(object obj) {
    if (!(obj is PlayerState)) return false;
    var other = (PlayerState)obj;
    bool equals = Position == other.Position;
    equals &= Velocity == other.Velocity;
    equals &= Direction == other.Direction;
    equals &= IsFastFalling == other.IsFastFalling;
    equals &= RemainingJumps == other.RemainingJumps;
    equals &= RespawnTimeRemaining == other.RespawnTimeRemaining;
    equals &= StateHash == other.StateHash;
    equals &= NormalizedStateTime == other.NormalizedStateTime;
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
      hash &= 919 * Velocity.GetHashCode();
      hash &= 373 * Direction.GetHashCode();
      hash &= 199 * IsFastFalling.GetHashCode();
      hash &= 131 * RemainingJumps.GetHashCode();
      hash &= 101 * RespawnTimeRemaining.GetHashCode();
      hash &= 83 *StateHash;
      hash &= 71 * NormalizedStateTime.GetHashCode();
      hash &= 59 * ShieldDamage.GetHashCode();
      hash &= 47 * ShieldRecoveryCooldown.GetHashCode();
      hash &= 43 * GrabbedLedgeID;
      hash &= 31 * Damage.GetHashCode();
      hash &= 17 * Hitstun.GetHashCode();
      hash &= Stocks;
      return hash;
    }
  }
}

}