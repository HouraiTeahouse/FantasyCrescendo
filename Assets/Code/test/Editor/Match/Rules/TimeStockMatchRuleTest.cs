using HouraiTeahouse.FantasyCrescendo;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;

public class TimeStockMatchRuleTest {

  GameState CreateGameState(uint time, int[] stocks) {
    var states = stocks.Select(s => new PlayerState { Stocks = s }).ToArray();
    return new GameState { Time = time, PlayerStates = states };
  }

	[Test]
	public void Simulate_each_simulate_decreases_remaining_time() {
    var timeMatchRule = new TimeStockMatchRule();
    var config = new GameConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new GameState (config) { Time = 100 };
    var input = new GameInput(config);
    var inputContext = new GameInputContext(input);

    Assert.AreEqual(99, timeMatchRule.Simulate(state, inputContext).Time);
	}

	[Test]
	public void GetResolution_continues_game_with_remaining_time() {
    var timeMatchRule = new TimeStockMatchRule();
    var config = new GameConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new GameState (config) { Time = 100 };

    Assert.AreEqual(null, timeMatchRule.GetResolution(state));
	}

	[Test]
	public void GetResolution_zero_time_results_in_timeout() {
    var timeMatchRule = new TimeStockMatchRule();
    var config = new GameConfig { Stocks = 5, PlayerConfigs = new PlayerConfig[4] };
    var state = new GameState (config) { Time = 0 };

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
