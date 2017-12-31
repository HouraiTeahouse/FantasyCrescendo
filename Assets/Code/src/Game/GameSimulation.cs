using HouraiTeahouse.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameSimulation : IInitializable<GameConfig>, ISimulation<GameState, GameInputContext> {

  PlayerSimulation[] PlayerSimulations;

  public ITask Initialize(GameConfig config) {
    Assert.IsTrue(config.IsValid);
    PlayerSimulations = new PlayerSimulation[config.PlayerConfigs.Length];
    var tasks = new List<ITask>();
    for (int i = 0; i < PlayerSimulations.Length; i++) {
      PlayerSimulations[i] = new PlayerSimulation();
      tasks.Add(PlayerSimulations[i].Initialize(config.PlayerConfigs[i]));
    }
    return Task.All(tasks);
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
    return newState;
  }

}


}
