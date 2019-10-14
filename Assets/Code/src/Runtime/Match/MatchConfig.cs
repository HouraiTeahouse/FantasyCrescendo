using HouraiTeahouse.FantasyCrescendo.Networking;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// A data object for configuring a game between multiple players.
/// </summary>
[Serializable]
public struct MatchConfig : IValidatable, INetworkSerializable {

  /// <summary>
  /// The ID of the stage that the match will be played on.
  /// </summary>
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
  PlayerConfig[] _playerConfigs;
  public ref PlayerConfig this[int playerId] => ref _playerConfigs[playerId];

  /// <summary>
  /// Gets the number of participating players in the game.
  /// </summary>
  public int PlayerCount => _playerConfigs.Length;

  public bool IsLocal {
    get {
      foreach (var config in _playerConfigs) {
        if (!config.IsLocal) return false;
      }
      return true;
    }
  }

  public bool IsValid { 
    get {
      foreach (var config in _playerConfigs) {
        if (!config.IsValid) return false;
      }
      return true;
    }
  }

  public override bool Equals(object obj) {
    if (typeof(MatchConfig) != obj.GetType()) return false;
    var other = (MatchConfig)obj;
    var equal = StageID == other.StageID;
    equal &= Time == other.Time;
    equal &= ArrayUtil.AreEqual(_playerConfigs, other._playerConfigs);
    return equal;
  }

  public override int GetHashCode() => unchecked(StageID.GetHashCode() * 31 + Time.GetHashCode() * 17 + ArrayUtil.GetOrderedHash(_playerConfigs));

  public PlayerConfig[] GetPlayerConfigs() => _playerConfigs;
  public void SetPlayerConfigs(IEnumerable<PlayerConfig> configs) => _playerConfigs = configs.ToArray();

  public void Serialize(Serializer serializer) {
    serializer.Write(StageID);
    serializer.Write(Stocks);
    serializer.Write(Time);
    serializer.Write((uint)_playerConfigs.Length);
    for (var i = 0; i < _playerConfigs.Length; i++) {
      serializer.Write(_playerConfigs[i]);
    }
  }

  public void Deserialize(Deserializer deserializer) {
    StageID = deserializer.ReadUInt32();
    Stocks = deserializer.ReadUInt32();
    Time = deserializer.ReadUInt32();
    var length = deserializer.ReadUInt32();
    _playerConfigs = new PlayerConfig[length];
    for (var i = 0; i < _playerConfigs.Length; i++) {
      _playerConfigs[i] = deserializer.Read<PlayerConfig>();
    }
  }

  public override string ToString() {
    var builder = new StringBuilder($"(MatchConfig {{{PlayerCount}}}: ");
    for (var i = 0; i < PlayerCount; i++) {
      builder.Append(_playerConfigs[i].ToString()).Append(" ");
    }
    builder.Append(")");
    return builder.ToString();
  }

}

}
