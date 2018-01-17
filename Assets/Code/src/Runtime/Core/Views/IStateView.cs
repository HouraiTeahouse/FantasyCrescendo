using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A player-facing display for showing a state.
/// </summary>
public interface IStateView<S> {

  /// <summary>
  /// Alters the outward appearance of the view to match the d
  /// data represented by a state.
  /// </summary>
  /// <param name="state">the state to display.</param>
  void ApplyState(S state);

}

public static class CoreUtility {

  public static T Simulate<T, TInput>(this IEnumerable<ISimulation<T, TInput>> simulations, T state, TInput input) {
    foreach (var simulation in simulations) {
      state = simulation.Simulate(state, input);
    }
    return state;
  }

  public static void ApplyState<T>(this IEnumerable<IStateView<T>> views, T state) {
    foreach (var view in views) {
      view?.ApplyState(state);
    }
  }
  
  public static Task Initialize<T>(this GameObject gameObject, T config) {
    return gameObject.GetComponentsInChildren<IInitializable<T>>().Initialize(config);
  }

  public static Task Initialize<T>(this IEnumerable<IInitializable<T>> initializers, T config) {
    return Task.WhenAll(initializers.Select(init => init.Initialize(config)));
  }

  public static Task<IStateView<T>[]> CreateAllViews<T, TConfig>(TConfig config) {
    var factories = Object.FindObjectsOfType<ViewFactory<T, TConfig>>();
    return factories.CreateViews(config);
  }

  public static async Task<IStateView<T>[]> CreateViews<T, TConfig>(this IEnumerable<IViewFactory<T, TConfig>> factories,
                                                                    TConfig config) {
    var viewsTask = await Task.WhenAll(factories.Select(f => f.CreateViews(config)));
    return viewsTask.SelectMany(v => v).ToArray();
  }

}

}
