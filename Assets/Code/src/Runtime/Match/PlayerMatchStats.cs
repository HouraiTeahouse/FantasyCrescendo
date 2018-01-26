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

  public override bool Equals(object obj) {
    if (typeof(PlayerMatchStats) != obj.GetType()) return false;
    var other = (PlayerMatchStats)obj;
    var equal = Config.Equals(other.Config);
    equal &= ArrayUtil.AreEqual(Deaths, other.Deaths);
    equal &= ArrayUtil.AreEqual(Kills, other.Kills);
    equal &= SelfDestructs == other.SelfDestructs;
    equal &= DamageDealt == other.DamageTaken;
    return equal;
  }

  public override int GetHashCode() {
    unchecked {
      var hash = 113 * Config.GetHashCode();
      hash += 83 * ArrayUtil.GetOrderedHash(Deaths);
      hash += 43 * ArrayUtil.GetOrderedHash(Kills);
      hash += 31 * SelfDestructs.GetHashCode();
      hash += 17 * DamageDealt.GetHashCode();
      hash += DamageTaken.GetHashCode();
      return hash;
    }
  }

}

}