using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchPlayerSimulation : IMatchSimulation {

  public PlayerSimulation[] PlayerSimulations;

  public Task Initialize(MatchConfig config) {
    Assert.IsTrue(config.IsValid);
    PlayerSimulations = new PlayerSimulation[config.PlayerCount];
    var tasks = new List<Task>();
    for (int i = 0; i < PlayerSimulations.Length; i++) {
      PlayerSimulations[i] = new PlayerSimulation();
      tasks.Add(PlayerSimulations[i].Initialize(config.PlayerConfigs[i]));
    }
    return Task.WhenAll(tasks);
  }

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    Assert.IsTrue(input.IsValid);
    Assert.AreEqual(PlayerSimulations.Length, state.PlayerCount);
    Assert.AreEqual(PlayerSimulations.Length, input.PlayerInputs.Length);
    var newState = state.Clone();
    for (uint i = 0; i < state.PlayerCount; i++) {
      PlayerSimulations[i].Presimulate(state.GetPlayerState(i));
    }
    for (uint i = 0; i < state.PlayerCount; i++) {
      newState.SetPlayerState(i, PlayerSimulations[i].Simulate(state.GetPlayerState(i), input.PlayerInputs[i]));
    }
    return newState;
  }

  public MatchState ResetState(MatchState state) {
    for (uint i = 0; i < state.PlayerCount; i++) {
      state.SetPlayerState(i, PlayerSimulations[i].ResetState(state.GetPlayerState(i)));
    }
    return state;
  }

  public void Dispose() { }

}

}
