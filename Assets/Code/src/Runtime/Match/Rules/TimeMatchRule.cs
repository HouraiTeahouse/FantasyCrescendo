using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public sealed class TimeMatchRule : MatchRule {

  public TimeMatchRule() : base() {
    Events.Subscribe<PlayerDiedEvent>(OnPlayerDied);
  }

  public override void Simulate(ref MatchState state,
                                in MatchInputContext input) {
    state.Time--;
  }

  public override MatchResolution? GetResolution(MatchState state) {
    if (state.Time <= 0) {
      return GetWinner(state) >= 0 ?
        MatchResolution.HasWinner :
        MatchResolution.Tie;
    }
    return null;
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
    evt.PlayerState.Stocks--;
    PlayerUtil.RespawnPlayer(evt);
  }

}

}
