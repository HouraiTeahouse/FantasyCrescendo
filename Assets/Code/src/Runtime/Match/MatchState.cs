using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// A complete representation of a given game's state at a given tick.
/// </summary>
[Serializable]
public class MatchState {

  public uint Time;

  PlayerState[] playerStates;
  public int PlayerCount => playerStates.Length;
  public IEnumerable<PlayerState> PlayerStates => playerStates.Select(x => x);

  public MatchState() {
    playerStates = new PlayerState[GameMode.GlobalMaxPlayers];
    UpdatePlayerStates();
  }

  public MatchState(int playerCount) {
    playerStates = new PlayerState[playerCount];
    UpdatePlayerStates();
  }

  public MatchState(IEnumerable<PlayerState> playerStates) {
    this.playerStates = playerStates.ToArray();
    UpdatePlayerStates();
  }

  /// <summary>
  /// Constructs a new GameState based on a given GameConfig.
  /// </summary>
  /// <param name="config">the configuration for the game.</param>
  public MatchState(MatchConfig config) {
    playerStates = new PlayerState[config.PlayerCount];
    Time = config.Time;
    for (var i = 0; i < playerStates.Length; i++) {
      playerStates[i].Stocks = (int)config.Stocks;
      playerStates[i].MatchState = this;
    }
  }

  /// <summary>
  /// Creates a deep clone of the state.
  /// </summary>
  /// <returns>a deep cloned copy of the state.</returns>
  public MatchState Clone() {
    MatchState clone = this;
    clone.playerStates = (PlayerState[]) playerStates.Clone();
    return clone;
  }

  public PlayerState GetPlayerState(uint index) => playerStates[index];
  public void SetPlayerState(uint index, PlayerState state) {
    state.MatchState = this;
    playerStates[index] = state;
  }

  public override bool Equals(object obj) {
    if (typeof(MatchState) != obj.GetType()) return false;
    var other = (MatchState)obj;
    return Time == other.Time && ArrayUtil.AreEqual(playerStates, other.playerStates);
  }

  public override int GetHashCode() {
    int hash = Time.GetHashCode();
    for (var i = 0; i < playerStates.Length; i++) {
      hash ^= playerStates[i].GetHashCode();
    }
    return hash;
  }

  void UpdatePlayerStates() {
    for (var i = 0; i < playerStates.Length; i++) {
      playerStates[i].MatchState = this;
    }
  }

}

}
