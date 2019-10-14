using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// Match Rule for normal stock matches. Players have a limited number of lives.
/// After expending all lives, they will no longer respawn. Last player alive
/// will be declared the winner.
/// </summary>
public class StockMatchRule : MatchRule {

  public StockMatchRule() : base() {
    Events.Subscribe<PlayerDiedEvent>(OnPlayerDied);
  }

  public override MatchResolution? GetResolution(MatchState state) {
    var livingCount = 0;
    for (var i = 0; i < state.PlayerCount; i++) {
      if (state[i].Stocks <= 0) continue;
      livingCount++;
    }
    switch(livingCount) {
      case 0: return MatchResolution.Tie;
      case 1: return MatchResolution.HasWinner;
      default: return null;
    }
  }

  public override int GetWinner(MatchState state) {
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
    ref var state = ref evt.PlayerState;
    state.Stocks = (sbyte)Mathf.Max(0, state.Stocks - 1);
    if (state.Stocks > 0) {
      PlayerUtil.RespawnPlayer(evt);
    }
  }

}

}
