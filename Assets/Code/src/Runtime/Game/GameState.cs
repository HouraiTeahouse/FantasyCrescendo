using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A complete representation of a given game's state at a given tick.
/// </summary>
public struct GameState {

  public PlayerState[] PlayerStates;

  /// <summary>
  /// Constructs a new GameState based on a given GameConfig.
  /// </summary>
  /// <param name="config">the configuration for the game.</param>
  public GameState(GameConfig config) {
    PlayerStates = new PlayerState[config.PlayerCount];
  }

  /// <summary>
  /// Creates a deep clone of the state.
  /// </summary>
  /// <returns>a deep cloned copy of the state.</returns>
  public GameState Clone() {
    GameState clone = this;
    clone.PlayerStates = (PlayerState[]) PlayerStates.Clone();
    return clone;
  }

}

}
