using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
    
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

}