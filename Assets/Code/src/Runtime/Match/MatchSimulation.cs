using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public interface IMatchSimulation : IInitializable<MatchConfig>, ISimulation<MatchState, MatchInputContext>, IDisposable {
  MatchState ResetState(MatchState state);
}

public class MatchSimulation : IMatchSimulation {

  readonly IMatchSimulation[] SimulationComponents;

  public MatchSimulation(IEnumerable<IMatchSimulation> simulationComponents) {
    SimulationComponents = simulationComponents.ToArray();
  }

  public Task Initialize(MatchConfig config) {
    return Task.WhenAll(SimulationComponents.Select(comp => comp.Initialize(config)));
  }

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    return SimulationComponents.Simulate(state, input);
  }

  public MatchState ResetState(MatchState state) {
    foreach (var component in SimulationComponents) {
      state = component.ResetState(state);
    }
    return state;
  }

  public void Dispose() {
    foreach (var component in SimulationComponents) {
      component.Dispose();
    }
  }

}
    
}
