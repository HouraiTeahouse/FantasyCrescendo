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
public abstract class AggregateBuilder<T, TBuilder> where TBuilder : class {

  readonly List<T> _subitems;

  protected AggregateBuilder() {
    _subitems = new List<T>();
  }

  public TBuilder AddSubitem(T view) {
    _subitems.Add(view);
    return this as TBuilder;
  }

  public TBuilder AddSubitems(IEnumerable<T> view) {
    _subitems.AddRange(view);
    return this as TBuilder;
  }

  public T Build() => BuildImpl(_subitems);

  protected abstract T BuildImpl(IEnumerable<T> subitems);

}

/// <summary>
/// Builder object for <see cref="AggregateView{T}"/> objects.
/// </summary>
/// <typeparam name="T">the state type for the view to build</typeparam>
public class ViewBuilder<T> : AggregateBuilder<IStateView<T>, ViewBuilder<T>> {

  protected override IStateView<T> BuildImpl(IEnumerable<IStateView<T>> views) => new AggregateView<T>(views);

}

/// <summary>
/// Builder object for <see cref="AggregateSimulation{TState, TContext}"/> objects.
/// </summary>
/// <typeparam name="TState">the target state type</typeparam>
/// <typeparam name="TContext">the context for the simulation</typeparam>
public class SimulationBuilder<TState, TContext> : AggregateBuilder<ISimulation<TState, TContext>, 
                                                                    SimulationBuilder<TState, TContext>> {

  protected override ISimulation<TState, TContext> BuildImpl(IEnumerable<ISimulation<TState, TContext>> simulations) => 
    new AggregateSimulation<TState, TContext>(simulations);

}

}