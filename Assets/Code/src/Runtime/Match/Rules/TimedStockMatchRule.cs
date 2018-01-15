using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public class TimeStockMatchRule : StockMatchRule {

  public override MatchState Simulate(MatchState state, MatchInputContext input) {
    state.Time--;
    return state;
  }

  public override MatchResolution? GetResolution(MatchState state) {
    MatchResolution? resolution = null;
    if (state.Time <= 0) {
      return GetWinner(state) != null ? MatchResolution.HasWinner : MatchResolution.Tie;
    }
    return base.GetResolution(state) ?? resolution;
  }

}

}