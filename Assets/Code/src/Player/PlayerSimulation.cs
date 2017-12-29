using HouraiTeahouse.Tasks;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerSimulation : IInitializable<PlayerConfig>, ISimulation<PlayerState, PlayerInput> {

  GameObject Model;
  ICharacterSimulation[] PresimulateComponents;
  ISimulation<PlayerState, PlayerInput>[] SimulationComponents;

  public ITask Initialize(PlayerConfig config) {
    var selection = config.Selection;
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    Model = Object.Instantiate(character.Prefab);
    Model.name = string.Format("Player {0} Simulation ({1}, {2})",
                               config.PlayerID + 1, character.name,
                               selection.Pallete);

    PlayerUtil.DestroyAll(Model, typeof(Renderer), typeof(MeshFilter));

    var task = Model.Broadcast<ICharacterComponent>(
        component => component.Initialize(config, false));

    SimulationComponents = Model.GetComponentsInChildren<ISimulation<PlayerState, PlayerInput>>();
    PresimulateComponents = SimulationComponents.OfType<ICharacterSimulation>()
                                                .ToArray();

    return task;
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
