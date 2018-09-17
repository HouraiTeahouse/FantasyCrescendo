using System;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// A top-level controller and manager of a multiplayer game's state.
/// </summary>
public interface IMatchController : IDisposable {

  /// <summary>
  /// Gets the current tick number. This should be monotonically increasing over time.
  /// </summary>
  uint Timestep { get; set; }

	ISimulation<MatchState, MatchInputContext> Simulation { get; set; }

	IMatchInputSource InputSource { get; set; }

  /// <summary>
  /// The current game state of the game.
  /// </summary>
  MatchState CurrentState { get; set; }

  /// <summary>
  /// Update the game state. Expected to be called once per Unity FixedUpdate.
  /// </summary>
  void Update();

}

}
