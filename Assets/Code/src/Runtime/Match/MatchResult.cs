using HouraiTeahouse.Networking;
using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

[Serializable]
public struct MatchResult : INetworkSerializable {

  public MatchResolution Resolution;
  public int WinningPlayerID;
  public PlayerMatchStats[] PlayerStats;

  public void Serialize(ref Serializer serializer) {
  }
  public void Deserialize(ref Deserializer deserializer) {
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
