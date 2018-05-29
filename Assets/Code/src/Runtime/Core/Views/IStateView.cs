using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A player-facing display for showing a state.
/// </summary>
public interface IStateView<S> {

  // TODO(james7132): Change ref -> in when C# 7.2 is available.

  /// <summary>
  /// Alters the outward appearance of the view to match the d
  /// data represented by a state.
  /// </summary>
  /// <param name="state">the state to display.</param>
  void ApplyState(ref S state);

}

public static class CoreUtility {

  public static void Simulate<T, TInput>(this ISimulation<T, TInput>[] simulations, ref T state, TInput input) {
    foreach (var simulation in simulations) {
      simulation.Simulate(ref state, input);
    }
  }

  public static void ApplyState<T>(this IStateView<T>[] views, T state) {
    foreach (var view in views) {
      view?.ApplyState(ref state);
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
