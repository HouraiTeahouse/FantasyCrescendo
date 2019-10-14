using HouraiTeahouse.FantasyCrescendo.Networking;
using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

[Serializable]
public struct MatchResult : INetworkSerializable {

  public MatchResolution Resolution;
  public int WinningPlayerID;
  public PlayerMatchStats[] PlayerStats;

  public override bool Equals(object obj) {
    if (typeof(MatchResult) != obj.GetType()) return false;
    var other = (MatchResult)obj;
    var equal = Resolution== other.Resolution;
    equal &= WinningPlayerID == other.WinningPlayerID;
    equal &= ArrayUtil.AreEqual(PlayerStats, other.PlayerStats);
    return equal;
  }

  public override int GetHashCode() {
    var hash = unchecked(Resolution.GetHashCode() * 31 + WinningPlayerID * 17);
    return hash + ArrayUtil.GetOrderedHash(PlayerStats);
  }

  public void Serialize(Serializer serializer) {
    // serializer.Write(MatchResult);
  }
  public void Deserialize(Deserializer deserializer) {
    // MatchResult = deserializer.Read<MatchResult>();
  }

}

public static class MatchResultUtil {

  public static PlayerMatchStats[] CreateMatchStatsFromConfig(MatchConfig config) {
    var players = new PlayerMatchStats[config.PlayerCount];
    for (var i = 0; i < players.Length; i++) {
      players[i].Config = config[i];
    }
    return players;
  }

}

}
