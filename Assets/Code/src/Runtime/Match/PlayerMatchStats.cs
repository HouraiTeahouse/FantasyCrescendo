using System;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct PlayerMatchStats {
  public PlayerConfig Config;

  public uint[] Deaths;
  public uint[] Kills;
  public uint SelfDestructs;

  public float DamageDealt;
  public float DamageTaken;

}

}