using HouraiTeahouse.Networking;
using HouraiTeahouse.Backroll;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public class EditorMatchConfig {
  public uint StageID;
  public uint Stocks;
  public uint Time;
  public int RandomSeed;
  public PlayerConfig[] PlayerConfigs;

  public MatchConfig CreateConfig() {
    var config = new MatchConfig {
      StageID = StageID,
      Stocks = Stocks,
      Time = Time,
      RandomSeed = Random.Range(int.MinValue, int.MaxValue),
      PlayerCount = (uint)PlayerConfigs.Length
    };
    for (var i = 0; i < PlayerConfigs.Length; i++) {
      config[i] = PlayerConfigs[i];
    }
    return config;
  }
}

/// <summary>
/// A data object for configuring a game between multiple players.
/// </summary>
[Serializable]
public unsafe struct MatchConfig : IValidatable, INetworkSerializable, IEnumerable<PlayerConfig> {

  const int kPlayerConfigSize = 11;

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
  /// Gets the number of participating players in the game.
  /// </summary>
  public uint PlayerCount;

  /// <summary>
  /// The initial random seed used when starting the match.
  /// </summary>
  public int RandomSeed;

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
  [SerializeField]
  fixed byte _playerConfigs[kPlayerConfigSize * BackrollConstants.kMaxPlayers];
  public ref PlayerConfig this[int id] {
    get {
      fixed (byte* ptr = _playerConfigs) {
        return ref ((PlayerConfig*)ptr)[id];
      }
    }
  }

  public bool IsLocal {
    get {
      if (PlayerCount <= 0) return false;
      for (var i = 0; i < PlayerCount; i++) {
        if (!this[i].IsLocal) return false;
      }
      return true;
    }
  }

  public bool IsValid { 
    get {
      if (PlayerCount <= 0) return false;
      for (var i = 0; i < PlayerCount; i++) {
        if (!this[i].IsValid) return false;
      }
      return true;
    }
  }

  public void Serialize(ref Serializer serializer) {
    serializer.Write(StageID);
    serializer.Write(Stocks);
    serializer.Write(Time);
    serializer.Write((uint)PlayerCount);
    for (var i = 0; i < PlayerCount; i++) {
      this[i].Serialize(ref serializer);
    }
  }

  public void Deserialize(ref Deserializer deserializer) {
    StageID = deserializer.ReadUInt32();
    Stocks = deserializer.ReadUInt32();
    Time = deserializer.ReadUInt32();
    PlayerCount = deserializer.ReadUInt32();
    for (var i = 0; i < PlayerCount; i++) {
      this[i].Deserialize(ref deserializer);
    }
  }

  public override string ToString() {
    var builder = new StringBuilder($"(MatchConfig {{{PlayerCount}}}: ");
    for (var i = 0; i < PlayerCount; i++) {
      builder.Append(this[i].ToString()).Append(" ");
    }
    builder.Append(")");
    return builder.ToString();
  }

  public IEnumerator<PlayerConfig> GetEnumerator() {
    for (var i = 0; i < PlayerCount; i++) {
      yield return this[i];
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

}
