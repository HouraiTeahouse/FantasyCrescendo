
namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A stateless simulation for advancing states through time given some external input.
/// </summary>
public interface ISimulation<S, I> {

  /// <summary>
  /// Advances a state by one timestep given the corresponding input.
  /// </summary>
  /// <param name="state">the previous timestep's state.</param>
  /// <param name="input">the input for the timestep.</param>
  /// <returns>the new state after a simulation timestep.</returns>
  S Simulate(S state, I input);

}

}
