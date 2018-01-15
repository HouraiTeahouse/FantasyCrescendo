using HouraiTeahouse.FantasyCrescendo;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;

public class StockMatchRuleTest {

  GameState CreateGameState(int[] stocks) {
    var states = stocks.Select(s => new PlayerState { Stocks = s }).ToArray();
    return new GameState { PlayerStates = states };
  }

	[Test]
	public void GetResolution_non_zero_stocks_does_not_end_match() {
    var stockRule = new StockMatchRule();
    Assert.AreEqual(null, stockRule.GetResolution(CreateGameState(new[] { 1, 2, 3, 4 })));
	}

	[Test]
	public void GetResolution_only_one_living_player_ends_match() {
    var stockRule = new StockMatchRule();
    Assert.AreEqual(MatchResolution.HasWinner, stockRule.GetResolution(CreateGameState(new[] { 1, 0, 0, 0 })));
    Assert.AreEqual(MatchResolution.HasWinner, stockRule.GetResolution(CreateGameState(new[] { 0, 5, 0, 0 })));
    Assert.AreEqual(MatchResolution.HasWinner, stockRule.GetResolution(CreateGameState(new[] { 0, 0, 2, 0 })));
    Assert.AreEqual(MatchResolution.HasWinner, stockRule.GetResolution(CreateGameState(new[] { 0, 0, 0, 3 })));
	}

	[Test]
	public void GetResolution_no_living_players_declares_tie() {
    var stockRule = new StockMatchRule();
    Assert.AreEqual(MatchResolution.Tie, stockRule.GetResolution(CreateGameState(new[] { 0, 0, 0, 0 })));
	}

	[Test]
	public void GetWinner_declares_winner_with_highest_stocks() {
    var stockRule = new StockMatchRule();
    Assert.AreEqual(0, stockRule.GetWinner(CreateGameState(new[] { 8, 2, 5, 6 })));
    Assert.AreEqual(1, stockRule.GetWinner(CreateGameState(new[] { 1, 5, 0, 4 })));
    Assert.AreEqual(2, stockRule.GetWinner(CreateGameState(new[] { 4, 2, 7, 0 })));
    Assert.AreEqual(3, stockRule.GetWinner(CreateGameState(new[] { 1, 2, 0, 5 })));
	}


}
