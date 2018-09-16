using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
    
public class AggregateView<T> : IStateView<T> {

  readonly IStateView<T>[] _subviews;

  public AggregateView(IEnumerable<IStateView<T>> subviews) {
    var flattenedSubviews = new List<IStateView<T>>();
    BuildSubviews(subviews, flattenedSubviews);
    _subviews = flattenedSubviews.ToArray();
  }

  public void UpdateView(in T state) {
    foreach (var view in _subviews) {
      view.UpdateView(state);
    }
  }

  static void BuildSubviews(IEnumerable<IStateView<T>> views, List<IStateView<T>> flattened) {
    foreach (var view in views) {
      if (view is AggregateView<T> aggregateView) {
        BuildSubviews(aggregateView._subviews, flattened);
      } else {
        flattened.Add(view);
      }
    }
  }

}

}