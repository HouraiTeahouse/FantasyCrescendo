using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public struct GameState {

  public PlayerState[] PlayerStates;

  public GameState(GameConfig config) {
    PlayerStates = new PlayerState[config.PlayerCount];
  }

  public GameState Clone() {
    GameState clone = this;
    clone.PlayerStates = (PlayerState[]) PlayerStates.Clone();
    return clone;
  }

}

}
