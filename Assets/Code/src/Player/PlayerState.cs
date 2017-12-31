using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

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

  public int StateHash;                               // 1-4 bytes
  public float NormalizedStateTime;                   // 4 bytes

  public float ShieldHealth;                          // 4 bytes
  public int ShieldRecoveryCooldown;                  // 1-4 bytes

  public float Damage;                                // 4 bytes
  public int Hitstun;                                 // 1-4 bytes

  public uint Stocks;                                 // 1-4 bytes

}

}
