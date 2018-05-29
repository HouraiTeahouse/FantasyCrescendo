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
  MediatorContext context;

  public Task Initialize(MatchConfig config) {
    Assert.IsTrue(config.IsValid);
    PlayerSimulations = new PlayerSimulation[config.PlayerCount];
    var tasks = new List<Task>();
    for (int i = 0; i < PlayerSimulations.Length; i++) {
      PlayerSimulations[i] = new PlayerSimulation();
      tasks.Add(PlayerSimulations[i].Initialize(config.PlayerConfigs[i]));
    }

    context = Mediator.Global.CreateContext();
    context.Subscribe<PlayerResetEvent>(ResetPlayer);

    return Task.WhenAll(tasks);
  }

  public void Simulate(ref MatchState state, MatchInputContext input) {
    Assert.IsTrue(input.IsValid);
    Assert.AreEqual(PlayerSimulations.Length, state.PlayerCount);
    Assert.AreEqual(PlayerSimulations.Length, input.PlayerInputs.Length);
    for (var i = 0; i < state.PlayerCount; i++) {
      PlayerSimulations[i].Presimulate(state.GetPlayerState(i));
    }
    for (var i = 0; i < state.PlayerCount; i++) {
      var playerState = state.GetPlayerState(i);
      var simulation = PlayerSimulations[i];
      var playerInput = input.PlayerInputs[i];
      simulation.Simulate(ref playerState, playerInput);
      state.SetPlayerState(i, playerState);
    }
  }

  public MatchState ResetState(MatchState state) {
    for (var i = 0; i < state.PlayerCount; i++) {
      // TODO(james7132): Replace this with ref return/locals
      var playerState = state.GetPlayerState(i);
      PlayerSimulations[i].ResetState(ref playerState);
      state.SetPlayerState(i, playerState);
    }
    return state;
  }

  void ResetPlayer(PlayerResetEvent evt) {
    var state = evt.PlayerState;
    PlayerSimulations[evt.PlayerID].ResetState(ref state);
    evt.PlayerState = state;
  }

  public void Dispose() => context.Dispose();

}

}
