
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

}
