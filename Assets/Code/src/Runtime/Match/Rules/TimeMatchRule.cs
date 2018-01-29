using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches.Rules {

public sealed class TimeMatchRule : IMatchRule {

  MediatorContext Events;

  public Task Initialize(MatchConfig config) {
    Events = Mediator.Global.CreateContext();
    Events.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    return Task.CompletedTask;
  }

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    state.Time--;
    return state;
  }

  public MatchResolution? GetResolution(MatchState state) {
    if (state.Time <= 0) {
      return GetWinner(state) != null ? MatchResolution.HasWinner : MatchResolution.Tie;
    }
    return null;
  }

  public uint? GetWinner(MatchState state) {
    uint? winner = null;
    int maxStocks = int.MinValue;
    for (uint i = 0; i < state.PlayerCount; i++) {
      var playerStocks = state.GetPlayerState(i).Stocks;
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

  void OnPlayerDied(PlayerDiedEvent evt) {
    var state = evt.PlayerState;
    state.Stocks--;
    evt.PlayerState = state;
    if (state.Stocks > 0) {
      PlayerUtil.RespawnPlayer(evt);
    }
  }


  public void Dispose() => Events?.Dispose();

}

}
