using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameSimulation : ISimulation<GameState, GameInput> {

  PlayerSimulation[] PlayerSimulations;
  HitboxSimulation hitboxSimulation;

  public GameSimulation(GameConfig config) {
    Assert.IsTrue(config.IsValid);
    PlayerSimulations = new PlayerSimulation[config.PlayerConfigs.Length];
    for (int i = 0; i < PlayerSimulations.Length; i++) {
      PlayerSimulations[i] = new PlayerSimulation(config.PlayerConfigs[i]);
    }

    hitboxSimulation = new HitboxSimulation(
        new Tuple<HitboxType, HitboxType>[]{
          Tuple.Create(HitboxType.Offensive, HitboxType.Shield),
          Tuple.Create(HitboxType.Offensive, HitboxType.Damageable)
        });
  }

  public GameState Simulate(GameState state, GameInput input) {
    Assert.IsTrue(input.IsValid);
    Assert.AreEqual(PlayerSimulations.Length, state.PlayerStates.Length);
    Assert.AreEqual(PlayerSimulations.Length, input.PlayerInputs.Length);

    var newState = state.Clone();

    using (HitboxSimulation.Context(hitboxSimulation)) {
      for (int i = 0; i < state.PlayerStates.Length; i++) {
        PlayerSimulations[i].Presimulate(state.PlayerStates[i]);
      }
      for (int i = 0; i < state.PlayerStates.Length; i++) {
        newState.PlayerStates[i] =
          PlayerSimulations[i].Simulate(state.PlayerStates[i], input.PlayerInputs[i]);
      }

      foreach (var collision in hitboxSimulation.Resolve()) {
        Debug.LogFormat("{0} {1}", collision.Source.Type, collision.Destination.Type);
      }
    }

    return newState;
  }

}


}
