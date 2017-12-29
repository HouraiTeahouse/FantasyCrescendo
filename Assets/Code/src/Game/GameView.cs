using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameView : IStateView<GameState> {

  public PlayerView[] PlayerViews;

  public GameView(GameConfig config) {
    PlayerViews = new PlayerView[config.PlayerConfigs.Length];
    for (int i = 0; i < PlayerViews.Length; i++) {
      PlayerViews[i] = new PlayerView(config.PlayerConfigs[i]);
    }
  }

  public void ApplyState(GameState state) {
    for (int i = 0; i < PlayerViews.Length; i++) {
      PlayerViews[i].ApplyState(state.PlayerStates[i]);
    }
  }

}

}
