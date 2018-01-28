using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerSimulation : IInitializable<PlayerConfig>, ISimulation<PlayerState, PlayerInputContext> {

  GameObject Model;
  IPlayerSimulation[] PlayerSimulationComponents;
  ISimulation<PlayerState, PlayerInputContext>[] SimulationComponents;

  public async Task Initialize(PlayerConfig config) {
    var selection = config.Selection;
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    var prefab = await character.Prefab.LoadAsync();
    Assert.IsNotNull(prefab);
    Model = Object.Instantiate(prefab);
    Model.name = $"Player {config.PlayerID + 1} Simulation ({character.name}, {selection.Pallete})";

    PlayerUtil.DestroyAll(Model, typeof(Renderer), typeof(MeshFilter));

    var task = Model.Broadcast<IPlayerComponent>(
        component => component.Initialize(config, false));

    SimulationComponents = Model.GetComponentsInChildren<ISimulation<PlayerState, PlayerInputContext>>();
    PlayerSimulationComponents = SimulationComponents.OfType<IPlayerSimulation>().ToArray();
    await task;
  }

  public void Presimulate(PlayerState state) {
    if (PlayerSimulationComponents == null) return;
    foreach (var component in PlayerSimulationComponents) {
      component.Presimulate(state);
    }
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    return SimulationComponents?.Simulate(state, input) ?? state;
  }

  public PlayerState ResetState(PlayerState state) {
    foreach (var component in PlayerSimulationComponents) {
      state = component.ResetState(state);
    }
    return state;
  }

}

}
