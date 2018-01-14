using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public class TimeStockMatchRule : StockMatchRule {

  public override GameState Simulate(GameState state, GameInput input) {
    state.Time--;
    return state;
  }

  public override MatchResolution? GetResolution(GameState state) {
    MatchResolution? resolution = null;
    if (state.Time <= 0) {
      resolution = MatchResolution.Timeout;
    }
    return base.GetResolution(state) ?? resolution;
  }

}

}