using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Matches.Rules {

/// <summary>
/// Match Rule for normal stock matches. Players have a limited number of lives.
/// After expending all lives, they will no longer respawn. Last player alive
/// will be declared the winner.
/// </summary>
public class StockMatchRule : IMatchRule {

  MediatorContext Events;

  public Task Initialize(MatchConfig config) {
    Events = Mediator.Global.CreateContext();
    Events.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    return Task.CompletedTask;
  }

  public virtual void Simulate(ref MatchState state, MatchInputContext input) {}

  public virtual MatchResolution? GetResolution(MatchState state) {
    var livingCount = 0;
    for (var i = 0; i < state.PlayerCount; i++) {
      if (state.GetPlayerState(i).Stocks <= 0) continue;
      livingCount++;
    }
    switch(livingCount) {
      case 0: return MatchResolution.Tie;
      case 1: return MatchResolution.HasWinner;
      default: return null;
    }
  }

  public virtual int GetWinner(MatchState state) {
    int winner = -1;
    int maxStocks = int.MinValue;
    for (var i = 0; i < state.PlayerCount; i++) {
      var playerStocks = state.GetPlayerState(i).Stocks;
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
    var state = evt.PlayerState;
    state.Stocks = (sbyte)Mathf.Max(0, state.Stocks - 1);
    evt.PlayerState = state;
    if (state.Stocks > 0) {
      PlayerUtil.RespawnPlayer(evt);
    }
  }

  public void Dispose() => Events?.Dispose();

}

}