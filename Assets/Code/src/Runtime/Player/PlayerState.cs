using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A complete representation of a player's state at a given tick.
/// </summary>
[Serializable]
public struct PlayerState {

  // One Player Total: 44 bytes
  // 4 Player: 176 bytes
  //
  // 60 times one: 2640 bytes
  // 60 times four: 10560 bytes

  // TODO(james7132): Generalize this
  public bool IsActive => Stocks > 0;
  public bool IsGrabbingLedge => GrabbedLedgeID != 0;
  public bool IsRespawning => RespawnTimeRemaining > 0;
  public bool IsHit => Hitstun > 0;

  public Vector2 Position;                            // 8 bytes
  public Vector2 Velocity;                            // 8 bytes

  // Direction: True => Right, False => Left
  public bool Direction;                              // One bit
  public bool IsFastFalling;                          // One bit

  public int RemainingJumps;                          // 1-4 bytes

  public uint RespawnTimeRemaining;                    // 1-4 bytes

  public int StateHash;                               // 1-4 bytes
  public float NormalizedStateTime;                   // 4 bytes

  public float ShieldHealth;                          // 4 bytes
  public int ShieldRecoveryCooldown;                  // 1-4 bytes

  public int GrabbedLedgeID;                          // 1 byte

  public float Damage;                                // 4 bytes
  public int Hitstun;                                 // 1-4 bytes

  public uint Stocks;                                 // 1-4 bytes

}

}