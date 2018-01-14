using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public class TimeStockMatchRule : StockMatchRule {

  public override GameState Simulate(GameState state, GameInputContext input) {
    state.Time--;
    return state;
  }

  public override MatchResolution? GetResolution(GameState state) {
    MatchResolution? resolution = null;
    if (state.Time <= 0) {
      return GetWinner(state) != null ? MatchResolution.HasWinner : MatchResolution.Tie;
    }
    return base.GetResolution(state) ?? resolution;
  }

}

}