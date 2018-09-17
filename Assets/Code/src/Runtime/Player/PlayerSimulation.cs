using System;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo.Players {

public class PlayerSimulation : IInitializable<PlayerConfig>, ISimulation<PlayerState, PlayerInputContext> {

  IPlayerSimulation[] PlayerSimulationComponents;
  ISimulation<PlayerState, PlayerInputContext>[] SimulationComponents;

  public async Task Initialize(PlayerConfig config) {
    var model = await PlayerUtil.Instantiate(config, false);

    var task = model.Broadcast<IPlayerComponent>(
        component => component.Initialize(config, false));

    SimulationComponents = model.GetComponentsInChildren<ISimulation<PlayerState, PlayerInputContext>>();
    PlayerSimulationComponents = SimulationComponents.OfType<IPlayerSimulation>().ToArray();
    await task;
  }

  public void Presimulate(in PlayerState state) {
    if (PlayerSimulationComponents == null) return;
    foreach (var component in PlayerSimulationComponents) {
      component.Presimulate(state);
    }
  }

  public void Simulate(ref PlayerState state, PlayerInputContext input) {
    // If under hitlag, do not further simulate player.
    if (state.Hitlag > 0) {
      state.Hitlag--;
      return;
    }
    SimulationComponents?.Simulate(ref state, input);
  }

  public void ResetState(ref PlayerState state) {
    if (PlayerSimulationComponents == null) return;
    foreach (var component in PlayerSimulationComponents) {
      if (component == null) continue;
      component.ResetState(ref state);
    }
  }

  public void Dispose() {
    if (PlayerSimulationComponents == null) return;
    foreach (var component in PlayerSimulationComponents) {
      if (component == null) continue;
      component.Dispose();
    }
  }

}

}
