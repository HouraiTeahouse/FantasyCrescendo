using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// A derivative of StockMatchRule that imposes a time limit. If the time limit
/// is reached. The player with the most lives remaining wins.
/// </summary>
public class TimeStockMatchRule : StockMatchRule {

  public override void Simulate(ref MatchState state,
                                in MatchInputContext input) {
    state.Time--;
  }

  public override MatchResolution? GetResolution(MatchState state) {
    MatchResolution? resolution = null;
    if (state.Time <= 0) {
      return GetWinner(state) >= 0 ?
        MatchResolution.HasWinner :
        MatchResolution.Tie;
    }
    return base.GetResolution(state) ?? resolution;
  }

}

}
