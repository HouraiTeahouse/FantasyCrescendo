using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerSimulation : ISimulation<PlayerState, PlayerInput> {

  GameObject Model;
  ICharacterSimulation[] PresimulateComponents;
  ISimulation<PlayerState, PlayerInput>[] SimulationComponents;

  public PlayerSimulation(PlayerConfig config) {
    var selection = config.Selection;
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    Model = Object.Instantiate(character.Prefab);
    Model.name = string.Format("Player {0} Simulation ({1}, {2})",
                               config.PlayerID + 1, character.name,
                               selection.Pallete);

    PlayerUtil.DestroyAll(Model, typeof(Renderer), typeof(MeshFilter));

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
