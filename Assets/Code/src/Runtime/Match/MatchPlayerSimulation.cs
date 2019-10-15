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

  readonly PlayerSimulation[] PlayerSimulations;
  readonly MediatorContext _context;

  public MatchPlayerSimulation(MatchConfig config) {
    Assert.IsTrue(config.IsValid);
    PlayerSimulations = new PlayerSimulation[config.PlayerCount];
    for (int i = 0; i < PlayerSimulations.Length; i++) {
      PlayerSimulations[i] = new PlayerSimulation();
    }

    _context = Mediator.Global.CreateContext();
    _context.Subscribe<PlayerResetEvent>(ResetPlayer);
  }

  public Task Initialize(MatchConfig config) {
    var tasks = new List<Task>();
    for (int i = 0; i < PlayerSimulations.Length; i++) {
      tasks.Add(PlayerSimulations[i].Initialize(config[i]));
    }
    return Task.WhenAll(tasks);
  }

  public void Simulate(ref MatchState state, in MatchInputContext input) {
    Assert.IsTrue(input.IsValid);
    Assert.AreEqual(PlayerSimulations.Length, state.PlayerCount);
    Assert.AreEqual(PlayerSimulations.Length, input.PlayerCount);
    for (var i = 0; i < state.PlayerCount; i++) {
      PlayerSimulations[i].Presimulate(state[i]);
    }
    for (var i = 0; i < state.PlayerCount; i++) {
      PlayerSimulations[i].Simulate(ref state[i], input[i]);
    }
  }

  public MatchState ResetState(MatchState state) {
    for (var i = 0; i < state.PlayerCount; i++) {
      PlayerSimulations[i].ResetState(ref state[i]);
    }
    return state;
  }

  void ResetPlayer(PlayerResetEvent evt) {
    PlayerSimulations[evt.PlayerID].ResetState(ref evt.PlayerState);
  }

  public void Dispose() => _context.Dispose();

}

}
