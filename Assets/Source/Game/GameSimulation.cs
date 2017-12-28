using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameSimulation : ISimulation<GameState, GameInput> {

  PlayerSimulation[] PlayerSimulations;

  public GameSimulation(GameConfig config) {
    Assert.IsTrue(config.IsValid);
    PlayerSimulations = new PlayerSimulation[config.PlayerConfigs.Length];
    for (int i = 0; i < PlayerSimulations.Length; i++) {
      PlayerSimulations[i] = new PlayerSimulation(config.PlayerConfigs[i]);
    }
  }

  public GameState Simulate(GameState state, GameInput input) {
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
