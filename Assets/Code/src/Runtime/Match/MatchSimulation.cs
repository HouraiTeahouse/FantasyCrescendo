using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public interface IMatchSimulation : IInitializable<MatchConfig>, ISimulation<MatchState, MatchInputContext>, IDisposable {
  MatchState ResetState(MatchState state);
}

public class MatchSimulation : IMatchSimulation {

  // Contains all simulations
  readonly IMatchSimulation[] SimulationComponents;
  // Contains simulations pertaining to the MatchProgressionState
  readonly Dictionary<MatchProgressionState, IMatchSimulation[]> Simulations;

  public MatchSimulation(IEnumerable<IMatchSimulation> simulationComponents) {
    SimulationComponents = simulationComponents.ToArray();

    Simulations = new Dictionary<MatchProgressionState, IMatchSimulation[]>(){
      { MatchProgressionState.Intro, CreateSimulations(SimulationComponents, typeof(MatchPlayerSimulation)) },
      { MatchProgressionState.InGame, SimulationComponents},
      { MatchProgressionState.End, CreateSimulations(SimulationComponents, typeof(MatchPlayerSimulation), typeof(MatchHitboxSimulation)) }
    };
  }

  public Task Initialize(MatchConfig config) {
    return Task.WhenAll(SimulationComponents.Select(comp => comp.Initialize(config)));
  }

  public void Simulate(ref MatchState state, in MatchInputContext input) {
    Simulations[state.StateID].Simulate(ref state, input);
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

  IMatchSimulation[] CreateSimulations(IMatchSimulation[] matchSimulations, params Type[] restrictions) {
    return matchSimulations.Where(s => restrictions.Contains(s.GetType())).ToArray();    
  }
  
}
    
}
