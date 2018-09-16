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

  public void Simulate(ref MatchState state, MatchInputContext input) {
    state.Time--;
  }

  public MatchResolution? GetResolution(MatchState state) {
    if (state.Time <= 0) {
      return GetWinner(state) >= 0 ? MatchResolution.HasWinner : MatchResolution.Tie;
    }
    return null;
  }

  public int GetWinner(MatchState state) {
    int winner = -1;
    int maxStocks = int.MinValue;
    for (var i = 0; i < state.PlayerCount; i++) {
      var playerStocks = state[i].Stocks;
      if (playerStocks > maxStocks) {
        winner = i;
        maxStocks = (int)playerStocks;
      } else if (playerStocks == maxStocks) {
        // More than one player alive. No current winner.
        winner = -1;
      }
    }
    return winner;
  }

  void OnPlayerDied(PlayerDiedEvent evt) {
    evt.PlayerState.Stocks--;
    if (evt.PlayerState.Stocks > 0) {
      PlayerUtil.RespawnPlayer(evt);
    }
  }


  public void Dispose() => Events?.Dispose();

}

}
