using HouraiTeahouse.FantasyCrescendo;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;

public class TimeMatchRuleTest {

  MatchState CreateGameState(uint time, int[] stocks) {
    var states = stocks.Select(s => new PlayerState { Stocks = s }).ToArray();
    return new MatchState(states) { Time = time };
  }

	[Test]
	public void Simulate_each_simulate_decreases_remaining_time() {
    var timeMatchRule = new TimeMatchRule();
    var config = new MatchConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new MatchState (config) { Time = 100 };
    var input = new MatchInput(config);
    var inputContext = new MatchInputContext(input);

    Assert.AreEqual(99, timeMatchRule.Simulate(state, inputContext).Time);
	}

	[Test]
	public void GetResolution_continues_game_with_remaining_time() {
    var timeMatchRule = new TimeMatchRule();
    var config = new MatchConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new MatchState (config) { Time = 100 };

    Assert.AreEqual(null, timeMatchRule.GetResolution(state));
	}

	[Test]
	public void GetResolution_zero_time_results_in_timeout() {
    var timeMatchRule = new TimeMatchRule();
    var config = new MatchConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new MatchState (config) { Time = 0 };

    Assert.AreEqual(MatchResolution.Tie, timeMatchRule.GetResolution(state));
	}


}
