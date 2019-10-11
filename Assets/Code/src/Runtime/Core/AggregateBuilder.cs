using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class AggregateObject<T> {

  protected ReadOnlyCollection<T> Subitems { get; }

  protected AggregateObject(IEnumerable<T> subitems) {
    var flattenedSubitems = new List<T>();
    FlattenSubitems(subitems, flattenedSubitems);
    Subitems = new ReadOnlyCollection<T>(flattenedSubitems.ToArray());
  }

  // Optimization: flattens subitems into a single enumeration to avoid traversing a tree during execution.
  static void FlattenSubitems(IEnumerable<T> views, List<T> flattened) {
    foreach (var view in views) {
      if (view is AggregateObject<T> aggregateObject) {
        FlattenSubitems(aggregateObject.Subitems, flattened);
      } else {
        flattened.Add(view);
      }
    }
  }

}
    
/// <summary>
/// Abstract class for building aggregate interfaces.
/// </summary>
/// <typeparam name="T">the interface to build</typeparam>
/// <typeparam name="TBuilder">self referential generic parameter</typeparam>
public abstract class AggregateBuilder<T> {

  readonly List<T> _subitems;

  protected AggregateBuilder() {
    _subitems = new List<T>();
  }

  public AggregateBuilder<T> AddSubitem(T view) {
    _subitems.Add(view);
    return this;
  }

  public AggregateBuilder<T> AddSubitems(IEnumerable<T> view) {
    _subitems.AddRange(view);
    return this;
  }

  public T Build() => BuildImpl(_subitems);

  protected abstract T BuildImpl(IEnumerable<T> subitems);

}

/// <summary>
/// Builder object for an aggregated view object. 
/// </summary>
/// <typeparam name="T">the state type for the view to build</typeparam>
public class ViewBuilder<T> : AggregateBuilder<IStateView<T>> {
    
  class AggregateView : AggregateObject<IStateView<T>>, IStateView<T> {

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

  protected override IStateView<T> BuildImpl(IEnumerable<IStateView<T>> views) => new AggregateView(views);

}

/// <summary>
/// Builder for an aggregated simulation object. Simulation children are executed in the order they are
/// added to the parent.
/// </summary>
/// <typeparam name="TState">the target state type</typeparam>
/// <typeparam name="TContext">the context for the simulation</typeparam>
public class SimulationBuilder<TState, TContext> : AggregateBuilder<ISimulation<TState, TContext>> {

  class AggregateSimulation : AggregateObject<ISimulation<TState, TContext>>,
                              ISimulation<TState, TContext> {

    public AggregateSimulation(IEnumerable<ISimulation<TState, TContext>> simulations) : base(simulations) {
    }

    public void Simulate(ref TState state, in TContext context) {
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

  protected override ISimulation<TState, TContext> BuildImpl(IEnumerable<ISimulation<TState, TContext>> simulations) => 
    new AggregateSimulation(simulations);

}

}