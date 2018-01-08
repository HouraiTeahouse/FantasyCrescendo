using HouraiTeahouse.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameView : IInitializable<GameConfig>, IStateView<GameState> {

  public PlayerView[] PlayerViews;

  public ITask Initialize(GameConfig config) {
    PlayerViews = new PlayerView[config.PlayerCount];
    var tasks = new List<ITask>();
    var viewFactories = Object.FindObjectsOfType<PlayerViewFactory>();
    for (int i = 0; i < PlayerViews.Length; i++) {
      PlayerViews[i] = new PlayerView(viewFactories);
      tasks.Add(PlayerViews[i].Initialize(config.PlayerConfigs[i]));
    }
    return Task.All(tasks);
  }

  public void ApplyState(GameState state) {
    for (int i = 0; i < PlayerViews.Length; i++) {
      PlayerViews[i].ApplyState(state.PlayerStates[i]);
    }
  }

}

}
