using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
    
public class AggregateView<T> : AggregateObject<IStateView<T>>, IStateView<T> {

  public AggregateView(IEnumerable<IStateView<T>> views) : base(views) {
  }

  public void UpdateView(in T state) {
    foreach (var view in Subitems) {
      if (view == null) continue;
      view.UpdateView(state);
    }
  }

  public void Dispose() {
    foreach (var view in Subitems) {
      if (view == null) continue;
      view.Dispose();
    }
  }

}

public class AggregateSimulation<TState, TContext> : AggregateObject<ISimulation<TState, TContext>>,
                                                     ISimulation<TState, TContext> {

  public AggregateSimulation(IEnumerable<ISimulation<TState, TContext>> simulations) : base(simulations) {
  }

  public void Simulate(ref TState state, TContext context) {
    foreach (var simulation in Subitems) {
      if (simulation == null) continue;
      simulation.Simulate(ref state, context);
    }
  }

  public void Dispose() {
    foreach (var simulation in Subitems) {
      if (simulation == null) continue;
      simulation.Dispose();
    }
  }

}

}