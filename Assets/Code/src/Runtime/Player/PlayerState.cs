using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A complete representation of a player's state at a given tick.
/// </summary>
public struct PlayerState {

  // One Player Total: 44 bytes
  // 4 Player: 176 bytes
  //
  // 60 times one: 2640 bytes
  // 60 times four: 10560 bytes

  public Vector2 Position;                            // 8 bytes
  public Vector2 Velocity;                            // 8 bytes

  // Direction: True => Right, False => Left
  public bool Direction;                              // One bit
  public bool IsFastFalling;                          // One bit

  public int RemainingJumps;                          // 1-4 bytes

  public int RespawnTimeRemaining;                    // 1-4 bytes

  public int StateHash;                               // 1-4 bytes
  public float NormalizedStateTime;                   // 4 bytes

  public float ShieldHealth;                          // 4 bytes
  public int ShieldRecoveryCooldown;                  // 1-4 bytes

  public int GrabbedLedgeID;                          // 1 byte

  public float Damage;                                // 4 bytes
  public int Hitstun;                                 // 1-4 bytes

  public uint Stocks;                                 // 1-4 bytes

  public bool IsGrabbingLedge => GrabbedLedgeID != 0;
  public bool IsHit => Hitstun > 0;

}

}