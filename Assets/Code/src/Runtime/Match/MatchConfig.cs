using HouraiTeahouse.FantasyCrescendo.Networking;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// A data object for configuring a game between multiple players.
/// </summary>
[Serializable]
public struct MatchConfig : IValidatable, INetworkSerializable {

  public uint StageID;

  /// <summary>
  /// The number of stocks each player starts off with. If set to zero, the 
  /// match will not be a stock match.
  /// </summary>
  public uint Stocks;

  /// <summary>
  /// The amount of time the match will last for in ticks. If zero the game
  /// will not have a time limit.
  /// </summary>
  public uint Time;

  /// <summary>
  /// Individual configurations for each participating player.
  /// </summary>
  /// <remarks>
  /// Note that each player's player ID does not directly correspond with
  /// the array index for the player's config. All players in the game 
  /// with a valid configuration are assumed to be active. For example, 
  /// the player at index 1 may not be P2. Player 2 may be inactive and 
  /// the player may be P3 or P4 instead.
  /// </remarks>
  public PlayerConfig[] PlayerConfigs;

  /// <summary>
  /// Gets the number of participating players in the game.
  /// </summary>
  public int PlayerCount => PlayerConfigs.Length;

  public bool IsLocal {
    get {
      bool isLocal = true;
      for (var i = 0; i < PlayerConfigs.Length; i++) {
        isLocal &= PlayerConfigs[i].IsLocal;
      }
      return isLocal;
    }
  }

  public bool IsValid => PlayerConfigs.IsAllValid();

  public override bool Equals(object obj) {
    if (typeof(MatchConfig) != obj.GetType()) return false;
    var other = (MatchConfig)obj;
    var equal = StageID == other.StageID;
    equal &= Time == other.Time;
    equal &= ArrayUtil.AreEqual(PlayerConfigs, other.PlayerConfigs);
    return equal;
  }

  public override int GetHashCode() => unchecked(StageID.GetHashCode() * 31 + Time.GetHashCode() * 17 + ArrayUtil.GetOrderedHash(PlayerConfigs));

  public void Serialize(Serializer serializer) {
    serializer.Write(StageID);
    serializer.Write(Stocks);
    serializer.Write(Time);
    serializer.Write((uint)PlayerConfigs.Length);
    for (var i = 0; i < PlayerConfigs.Length; i++) {
      serializer.Write(PlayerConfigs[i]);
    }
  }

  public void Deserialize(Deserializer deserializer) {
    StageID = deserializer.ReadUInt32();
    Stocks = deserializer.ReadUInt32();
    Time = deserializer.ReadUInt32();
    var length = deserializer.ReadUInt32();
    PlayerConfigs = new PlayerConfig[length];
    for (var i = 0; i < PlayerConfigs.Length; i++) {
      PlayerConfigs[i] = deserializer.Read<PlayerConfig>();
    }
  }

  public override string ToString() {
    var builder = new StringBuilder($"(MatchConfig {{{PlayerCount}}}: ");
    for (var i = 0; i < PlayerCount; i++) {
      builder.Append(PlayerConfigs[i].ToString()).Append(" ");
    }
    builder.Append(")");
    return builder.ToString();
  }

}

}
