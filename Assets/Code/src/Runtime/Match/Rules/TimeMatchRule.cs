using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public sealed class TimeMatchRule : IMatchRule {

  public GameState Simulate(GameState state, GameInputContext input) {
    state.Time--;
    return state;
  }

  public MatchResolution? GetResolution(GameState state) {
    if (state.Time <= 0) {
      return GetWinner(state) != null ? MatchResolution.HasWinner : MatchResolution.Tie;
    }
    return null;
  }

  public uint? GetWinner(GameState state) {
    uint? winner = null;
    int maxStocks = int.MinValue;
    for (uint i = 0; i < state.PlayerStates.Length; i++) {
      var playerStocks = state.PlayerStates[i].Stocks;
      if (playerStocks > maxStocks) {
        winner = i;
        maxStocks = (int)playerStocks;
      } else if (playerStocks == maxStocks) {
        // More than one player alive. No current winner.
        winner = null;
      }
    }
    return winner;
  }

}

}
