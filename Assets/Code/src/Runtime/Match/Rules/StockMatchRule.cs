using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// Match Rule for normal stock matches. Players have a limited number of lives.
/// After expending all lives, they will no longer respawn. Last player alive
/// will be declared the winner.
/// </summary>
public class StockMatchRule : IMatchRule {

  public Task Initalize(GameConfig config) => Task.CompletedTask;

  public virtual GameState Simulate(GameState state, GameInputContext input) {
    return state;
  }

  public virtual MatchResolution? GetResolution(GameState state) {
    var livingCount = state.PlayerStates.Count(player => player.Stocks > 0);
    switch(livingCount) {
      case 0: return MatchResolution.Tie;
      case 1: return MatchResolution.HasWinner;
      default: return null;
    }
  }

  public virtual uint? GetWinner(GameState state) {
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