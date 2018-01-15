using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameView : IInitializable<GameConfig>, IStateView<GameState> {

  public PlayerView[] PlayerViews;

  public Task Initialize(GameConfig config) {
    PlayerViews = new PlayerView[config.PlayerCount];
    var tasks = new List<Task>();
    var viewFactories = Object.FindObjectsOfType<AbstractViewFactory<PlayerState, PlayerConfig>>();
    for (int i = 0; i < PlayerViews.Length; i++) {
      PlayerViews[i] = new PlayerView(viewFactories);
      tasks.Add(PlayerViews[i].Initialize(config.PlayerConfigs[i]));
    }
    return Task.WhenAll(tasks);
  }

  public void ApplyState(GameState state) {
    for (int i = 0; i < PlayerViews.Length; i++) {
      PlayerViews[i].ApplyState(state.PlayerStates[i]);
    }
  }

}

}
