using System;
using System.Collections;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

public static class MatchRuleFactory {

  public static IEnumerable<IMatchRule> CreateRules(MatchConfig config) {
    var stock = config.Stocks > 0;
    var time = config.Time > 0;
    if (stock && time) {
      yield return new TimeStockMatchRule();
    } else if (stock) {
      yield return new StockMatchRule();
    } else if (time) {
      yield return new TimeMatchRule();
    } else {
      throw new ArgumentException("Invalid MatchConfig, no Match Rules created.");
    }
  }

}
    
}
