using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Matches.Rules;
using HouraiTeahouse.FantasyCrescendo.Players;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;

public class TimeStockMatchRuleTest {

  MatchState CreateGameState(uint time, int[] stocks) {
    var states = stocks.Select(s => new PlayerState { Stocks = (sbyte)s });
    return new MatchState(states) { Time = time };
  }

	[Test]
	public void Simulate_each_simulate_decreases_remaining_time() {
    var timeMatchRule = new TimeStockMatchRule();
    var config = new MatchConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new MatchState (config) { Time = 100 };
    var input = new MatchInput(config);
    var inputContext = new MatchInputContext(input);

    Assert.AreEqual(99, timeMatchRule.Simulate(state, inputContext).Time);
	}

	[Test]
	public void GetResolution_continues_game_with_remaining_time() {
    var timeMatchRule = new TimeStockMatchRule();
    var config = new MatchConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new MatchState (config) { Time = 100 };

    Assert.AreEqual(null, timeMatchRule.GetResolution(state));
	}

	[Test]
	public void GetResolution_zero_time_results_in_timeout() {
    var timeMatchRule = new TimeStockMatchRule();
    var config = new MatchConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new MatchState (config) { Time = 0 };

    Assert.AreEqual(MatchResolution.Tie, timeMatchRule.GetResolution(state));
	}

	[Test]
	public void GetResolution_non_zero_stocks_does_not_end_match() {
    var stockRule = new TimeStockMatchRule();
    Assert.AreEqual(null, stockRule.GetResolution(CreateGameState(100, new[] { 1, 2, 3, 4 })));
	}

	[Test]
	public void GetResolution_only_one_living_player_ends_match() {
    var stockRule = new TimeStockMatchRule();
    Assert.AreEqual(MatchResolution.HasWinner, stockRule.GetResolution(CreateGameState(100, new[] { 1, 0, 0, 0 })));
    Assert.AreEqual(MatchResolution.HasWinner, stockRule.GetResolution(CreateGameState(100, new[] { 0, 5, 0, 0 })));
    Assert.AreEqual(MatchResolution.HasWinner, stockRule.GetResolution(CreateGameState(100, new[] { 0, 0, 2, 0 })));
    Assert.AreEqual(MatchResolution.HasWinner, stockRule.GetResolution(CreateGameState(100, new[] { 0, 0, 0, 3 })));
	}

	[Test]
	public void GetWinner_declares_winner_with_highest_stocks() {
    var stockRule = new TimeStockMatchRule();
    Assert.AreEqual(0, stockRule.GetWinner(CreateGameState(100, new[] { 8, 2, 5, 6 })));
    Assert.AreEqual(1, stockRule.GetWinner(CreateGameState(100, new[] { 1, 5, 0, 4 })));
    Assert.AreEqual(2, stockRule.GetWinner(CreateGameState(100, new[] { 4, 2, 7, 0 })));
    Assert.AreEqual(3, stockRule.GetWinner(CreateGameState(100, new[] { 1, 2, 0, 5 })));
	}



}
