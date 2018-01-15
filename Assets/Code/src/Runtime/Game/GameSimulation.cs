using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameSimulation : IInitializable<GameConfig>, ISimulation<GameState, GameInputContext>, IDisposable {

  public PlayerSimulation[] PlayerSimulations;

  //TODO(james7132): Move this to somewhere more sane.
  MediatorContext Events;
  BlastZone BlastZone;

  public Task Initialize(GameConfig config) {
    Assert.IsTrue(config.IsValid);
    Events = Mediator.Global.CreateContext();
    Events.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    BlastZone = Object.FindObjectOfType<BlastZone>();
    PlayerSimulations = new PlayerSimulation[config.PlayerCount];
    var tasks = new List<Task>();
    for (int i = 0; i < PlayerSimulations.Length; i++) {
      PlayerSimulations[i] = new PlayerSimulation();
      tasks.Add(PlayerSimulations[i].Initialize(config.PlayerConfigs[i]));
    }
    return Task.WhenAll(tasks);
  }

  public GameState Simulate(GameState state, GameInputContext input) {
    Assert.IsTrue(input.IsValid);
    Assert.AreEqual(PlayerSimulations.Length, state.PlayerStates.Length);
    Assert.AreEqual(PlayerSimulations.Length, input.PlayerInputs.Length);
    var newState = state.Clone();
    for (int i = 0; i < state.PlayerStates.Length; i++) {
      PlayerSimulations[i].Presimulate(state.PlayerStates[i]);
    }
    for (int i = 0; i < state.PlayerStates.Length; i++) {
      newState.PlayerStates[i] =
        PlayerSimulations[i].Simulate(state.PlayerStates[i], input.PlayerInputs[i]);
    }
    if (BlastZone != null) {
      newState = BlastZone.Simulate(newState);
    }
    return newState;
  }

  void OnPlayerDied(PlayerDiedEvent evt) {
    var state = evt.PlayerState;
    state.Stocks = Mathf.Max(0, state.Stocks - 1);
    evt.PlayerState = state;
    var shouldRespawn = state.Stocks > 0;
    if (!shouldRespawn) return;
    var respawnEvent = new PlayerRespawnEvent();
    respawnEvent.Copy(evt);
    Mediator.Global.Publish(respawnEvent);
    evt.Copy(respawnEvent);
  }

  public GameState ResetState(GameState state) {
    for (int i = 0; i < state.PlayerStates.Length; i++) {
      state.PlayerStates[i] = PlayerSimulations[i].ResetState(state.PlayerStates[i]);
    }
    return state;
  }

  public void Dispose() => Events?.Dispose();

}


}
