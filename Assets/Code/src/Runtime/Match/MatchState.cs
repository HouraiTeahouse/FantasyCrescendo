using System;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A complete representation of a given game's state at a given tick.
/// </summary>
[Serializable]
public struct MatchState {

  public uint Time;

  public PlayerState[] PlayerStates;

  /// <summary>
  /// Constructs a new GameState based on a given GameConfig.
  /// </summary>
  /// <param name="config">the configuration for the game.</param>
  public MatchState(MatchConfig config) {
    PlayerStates = new PlayerState[config.PlayerCount];
    Time = config.Time;
    for (var i = 0; i < PlayerStates.Length; i++) {
      PlayerStates[i].Stocks = (int)config.Stocks;
    }
  }

  /// <summary>
  /// Creates a deep clone of the state.
  /// </summary>
  /// <returns>a deep cloned copy of the state.</returns>
  public MatchState Clone() {
    MatchState clone = this;
    clone.PlayerStates = (PlayerState[]) PlayerStates.Clone();
    return clone;
  }

  public override bool Equals(object obj) {
    if (!(obj is MatchState)) return false;
    var state = (MatchState)obj;
    return Time == state.Time && Enumerable.SequenceEqual(PlayerStates, state.PlayerStates);
  }

  public override int GetHashCode() {
    int hash = Time.GetHashCode();
    for (var i = 0; i < PlayerStates.Length; i++) {
      hash ^= PlayerStates[i].GetHashCode();
    }
    return hash;
  }

}

}
