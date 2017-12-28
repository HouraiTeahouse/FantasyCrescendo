using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public struct PlayerState {

  public Vector2 Position;
  public Vector2 Velocity;

  // Direction: True => Right, False => Left
  public bool Direction;

  public int StateHash;
  public int NormalizedStateTime;

  public float Damage;
  public ushort Hitstun;

  public uint Stocks;

}

}
