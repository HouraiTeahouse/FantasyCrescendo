using HouraiTeahouse.Networking;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.FantasyCrescendo {

  /// <summary>
  /// Represents the match's state in game
  /// </summary>
public enum MatchProgressionState : byte { 
  /// <summary>
  /// Before the game begins. Represents the countdown sequence.
  /// </summary>
  Intro, 
  /// <summary>
  /// Represents the game in progress.
  /// </summary>
  InGame, 
  /// <summary>
  /// Represents the game while paused. Can only be paused during InGame.
  /// </summary>
  Pause, 
  /// <summary>
  /// After the game results are in. Represents the GAME/TIME sequence.
  /// </summary>
  End 
}

/// <summary>
/// A complete representation of a given game's state at a given tick.
/// </summary>
[Serializable]
public class MatchState : INetworkSerializable {

  public uint Time;
  public MatchProgressionState StateID = MatchProgressionState.Intro;
  public MatchInput LastInput;
  public Random.State RandomState;

  [SerializeField]
  PlayerState[] playerStates;
  public int PlayerCount { get; private set; }

  public MatchState() : this((int)GameMode.GlobalMaxPlayers) { }

  public MatchState(int playerCount) {
    PlayerCount = playerCount;
    playerStates = ArrayPool<PlayerState>.Shared.Rent((int)PlayerCount);
    UpdatePlayerStates();
  }

  public MatchState(IEnumerable<PlayerState> playerStates) {
    this.playerStates = playerStates.ToArray();
    PlayerCount = this.playerStates.Length;
    RandomState = Random.state;
    UpdatePlayerStates();
  }

  ~MatchState() {
    if (playerStates == null) return;
    ArrayPool<PlayerState>.Shared.Return(playerStates);
    playerStates = null;
  }

  /// <summary>
  /// Constructs a new GameState based on a given GameConfig.
  /// </summary>
  /// <param name="config">the configuration for the game.</param>
  public MatchState(MatchConfig config) : this((int)config.PlayerCount) {
    Time = config.Time;
    for (var i = 0; i < PlayerCount; i++) {
      playerStates[i].Stocks = (sbyte)config.Stocks;
      playerStates[i].MatchState = this;
    }
    Random.InitState(config.RandomSeed);
    RandomState = Random.state;
  }

  /// <summary>
  /// Creates a deep clone of the state.
  /// </summary>
  /// <returns>a deep cloned copy of the state.</returns>
  public MatchState Clone() {
    var clone = (MatchState)MemberwiseClone();
    clone.playerStates = ArrayPool<PlayerState>.Shared.Rent(PlayerCount);
    Array.Copy(playerStates, clone.playerStates, PlayerCount);
    return clone;
  }

  public void Serialize(ref Serializer serializer) {
    serializer.Write(Time);
    serializer.Write((byte)StateID);
    serializer.WriteStruct(ref LastInput);
    serializer.WriteStruct(ref RandomState);
    byte activeMask = (byte)(PlayerCount & 15);
    for (var i = 0; i < PlayerCount; i++) {
      if (!playerStates[i].IsActive) continue;
      activeMask |= (byte)(1 << (i + 4));
    }
    serializer.Write(activeMask);
    for (uint i = 0; i < PlayerCount; i++) {
      if (playerStates[i].IsActive) {
        playerStates[i].Serialize(ref serializer);
      }
    }
    serializer.Write((byte)StateID);
  }

  public void Deserialize(ref Deserializer deserializer) {
    Time = deserializer.ReadUInt32();
    StateID = (MatchProgressionState)deserializer.ReadByte();
    deserializer.ReadStruct(ref LastInput);
    deserializer.ReadStruct(ref RandomState);
    var mask = deserializer.ReadByte();
    PlayerCount = mask & 15;
    playerStates = ArrayPool<PlayerState>.Shared.Rent(PlayerCount);
    for (var i = 0; i < PlayerCount; i++) {
      var state = new PlayerState();
      if ((mask & (1 << (i + 4))) != 0) {
        state.Deserialize(ref deserializer);
      }
      state.MatchState = this;
      this[i] = state;
    }
    StateID = (MatchProgressionState)deserializer.ReadByte();
  }

  public ref PlayerState this[int idx] => ref playerStates[idx];

  public override string ToString() {
    var players = string.Join(" ", playerStates.Take(PlayerCount).Select(p => p.GetHashCode().ToString()));
    return $"{{MatchState ({PlayerCount}): {players}}}";
  }

  public override bool Equals(object obj) {
    var other = obj as MatchState;
    if (other == null) return false;
    if (Time != other.Time || PlayerCount != other.PlayerCount) return false;
    bool equal = true;
    for (var i = 0; i < PlayerCount; i++) {
      equal &= playerStates[i].Equals(other.playerStates[i]);
    }
    return equal;
  }

  public override int GetHashCode() {
    int hash = Time.GetHashCode();
    for (var i = 0; i < PlayerCount; i++) {
      hash ^= playerStates[i].GetHashCode();
    }
    return hash;
  }

  void UpdatePlayerStates() {
    for (var i = 0; i < PlayerCount; i++) {
      playerStates[i].MatchState = this;
    }
  }

}

}
