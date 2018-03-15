using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchView : IInitializable<MatchConfig>, IStateView<MatchState> {

  public IStateView<MatchState>[] MatchViews;
  public PlayerView[] PlayerViews;

  public async Task Initialize(MatchConfig config) {
    var playerInit = InitializePlayers(config);
    var otherInit = InitializeOtherViews(config);
    await Task.WhenAll(playerInit, otherInit);
  }

  async Task InitializePlayers(MatchConfig config) {
    PlayerViews = new PlayerView[config.PlayerCount];
    var tasks = new List<Task>();
    var viewFactories = Object.FindObjectsOfType<ViewFactory<PlayerState, PlayerConfig>>();
    for (int i = 0; i < PlayerViews.Length; i++) {
      PlayerViews[i] = new PlayerView(viewFactories);
      tasks.Add(PlayerViews[i].Initialize(config.PlayerConfigs[i]));
    }
    await Task.WhenAll(tasks);
  }

  async Task InitializeOtherViews(MatchConfig config) {
    MatchViews = await CoreUtility.CreateAllViews<MatchState, MatchConfig>(config);
  }

  public void ApplyState(MatchState state) {
    ApplyPlayerStates(state);
    ApplyOtherStates(state);
  }

  void ApplyPlayerStates(MatchState state) {
    for (var i = 0; i < PlayerViews.Length; i++) {
      PlayerViews[i].ApplyState(state.GetPlayerState(i));
    }
  }

  void ApplyOtherStates(MatchState state) {
    foreach (var view in MatchViews) {
      view.ApplyState(state);
    }
  }

}

}
