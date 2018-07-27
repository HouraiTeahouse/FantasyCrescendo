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

  public void Simulate(ref MatchState state, MatchInputContext input) {
	 if(state.StateID == MatchStateID.InGame)
		SimulationComponents.Simulate(ref state, input);
	 else if (state.StateID == MatchStateID.Intro)
		SimulateRestrict(ref state, input, typeof(MatchPlayerSimulation));
	 else if (state.StateID == MatchStateID.End)
		SimulateRestrict(ref state, input, typeof(MatchPlayerSimulation), typeof(MatchHitboxSimulation));
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

  void SimulateRestrict(ref MatchState state, MatchInputContext input, params Type[] restrictions) {
	 foreach (var SimComponent in SimulationComponents) {
		if (restrictions.Contains(SimComponent.GetType()))
		  SimComponent.Simulate(ref state, input);
  }
  
  }

  }
    
}
