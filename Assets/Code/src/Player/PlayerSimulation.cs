using HouraiTeahouse.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerSimulation : IInitializable<PlayerConfig>, ISimulation<PlayerState, PlayerInputContext> {

  GameObject Model;
  ICharacterSimulation[] PresimulateComponents;
  ISimulation<PlayerState, PlayerInputContext>[] SimulationComponents;

  public ITask Initialize(PlayerConfig config) {
    var selection = config.Selection;
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    return character.Prefab.LoadAsync().Then(prefab => {
      Assert.IsNotNull(prefab);
      Model = Object.Instantiate(prefab);
      Model.name = $"Player {config.PlayerID + 1} Simulation ({character.name}, {selection.Pallete})";

      PlayerUtil.DestroyAll(Model, typeof(Renderer), typeof(MeshFilter));

      var task = Model.Broadcast<ICharacterComponent>(
          component => component.Initialize(config, false));

      SimulationComponents = Model.GetComponentsInChildren<ISimulation<PlayerState, PlayerInputContext>>();
      PresimulateComponents = SimulationComponents.OfType<ICharacterSimulation>()
                                                  .ToArray();
      return task;
    });
  }

  public void Presimulate(PlayerState state) {
    foreach (var component in PresimulateComponents) {
      component.Presimulate(state);
    }
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    //Assert.IsTrue(input.IsValid);
    foreach (var component in SimulationComponents) {
      state = component.Simulate(state, input);
    }
    return state;
  }

}

}
