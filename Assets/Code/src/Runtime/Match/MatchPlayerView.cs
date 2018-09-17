using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchPlayerView : IInitializable<MatchConfig>, IStateView<MatchState> {

  IStateView<PlayerState>[] PlayerViews;

  public async Task Initialize(MatchConfig config) {
    PlayerViews = new IStateView<PlayerState>[config.PlayerCount];
    var tasks = new List<Task>();
    var factories = Object.FindObjectsOfType<ViewFactory<PlayerState, PlayerConfig>>();
    for (var i = 0; i < PlayerViews.Length; i++) {
      tasks.Add(CreatePlayerView(i, config.PlayerConfigs[i], factories));
    }
    await Task.WhenAll(tasks);
  }

  async Task CreatePlayerView(int id, PlayerConfig config, 
                              ViewFactory<PlayerState, PlayerConfig>[] factories) {
    var view = await PlayerUtil.Instantiate(config, true);

    var task = view.Broadcast<IPlayerComponent>(component => component.Initialize(config, true));

    var viewTasks = from factory in factories
                    where factory != null
                    select factory.CreateViews(config);
    var views = (await Task.WhenAll(viewTasks)).SelectMany(v => v);
    await task;

    // TODO(james7132): Move character view components to it's own factory
    PlayerViews[id] = new ViewBuilder<PlayerState>()
      .AddSubitems(view.GetComponentsInChildren<IStateView<PlayerState>>())
      .AddSubitems(views)
      .Build();
  }

  public void UpdateView(in MatchState state) {
    for (var i = 0; i < PlayerViews.Length; i++) {
      if (PlayerViews[i] == null) continue;
      PlayerViews[i].UpdateView(state[i]);
    }
  }

  public void Dispose() {
    foreach (var view in PlayerViews) {
      if (view == null) continue;
      view.Dispose();
    }
  }

}

}
