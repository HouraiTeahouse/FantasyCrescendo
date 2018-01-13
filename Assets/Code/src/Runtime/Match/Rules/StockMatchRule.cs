using HouraiTeahouse.Tasks;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// Match Rule for normal stock matches. Players have a limited number of lives.
/// After expending all lives, they will no longer respawn. Last player alive
/// will be declared the winner.
/// </summary>
public class StockMatchRule : IMatchRule {

  public ITask Initalize(GameConfig config) => Task.Resolved;

  public virtual GameState Simulate(GameState state, GameInput input) {
    return state;
  }

  public virtual MatchResolution? GetResolution(GameState state) {
    if (state.PlayerStates.Count(player => player.Stocks <= 0) <= 1){
      return MatchResolution.HasWinner;
    }
    return null;
  }

  public virtual uint? GetWinner(GameState state) {
    uint? winner = null;
    for (uint i = 0; i < state.PlayerStates.Length; i++) {
      if (state.PlayerStates[i].Stocks > 0) {
        if (winner == null) {
          winner = i;
        } else {
          // More than one player alive. No current winner.
          return null;
        }
      }
    }
    return winner;
  }

}

}