using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerSimulation : ISimulation<PlayerState, PlayerInput> {

  GameObject Model;
  ICharacterSimulation[] PresimulateComponents;
  ISimulation<PlayerState, PlayerInput>[] SimulationComponents;

  public PlayerSimulation(PlayerConfig config) {
    var character = Registry.Get<CharacterData>().Get(config.Selection.CharacterID);
    Model = Object.Instantiate(character.Prefab);
    Model.name = character.name + " (Player Simulation)";

    PlayerUtil.DestroyAll(Model, typeof(Renderer));

    foreach (var component in Model.GetComponentsInChildren<ICharacterComponent>()) {
      component.Initialize(config);
    }

    SimulationComponents = Model.GetComponentsInChildren<ISimulation<PlayerState, PlayerInput>>();
    PresimulateComponents = SimulationComponents.OfType<ICharacterSimulation>()
                                                .ToArray();
  }

  public void Presimulate(PlayerState state) {
    foreach (var component in PresimulateComponents) {
      component.Presimulate(state);
    }
  }

  public PlayerState Simulate(PlayerState state, PlayerInput input) {
    //Assert.IsTrue(input.IsValid);
    foreach (var component in SimulationComponents) {
      state = component.Simulate(state, input);
    }
    return state;
  }

}

}
