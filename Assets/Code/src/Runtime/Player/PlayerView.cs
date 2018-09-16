using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Players {

/// <summary>
/// Manages the visual display of a single player's state within a multiplayer match.
/// </summary>
public class PlayerView : IInitializable<PlayerConfig>, IStateView<PlayerState> {

  IStateView<PlayerState>[] ViewComponents;

  readonly IEnumerable<IViewFactory<PlayerState, PlayerConfig>> viewFactories;

  public PlayerView(IEnumerable<IViewFactory<PlayerState, PlayerConfig>> viewFactories) {
    this.viewFactories = viewFactories;
  }

  public async Task Initialize(PlayerConfig config) {
    var view = await PlayerUtil.Instantiate(config, true);

    var task = view.Broadcast<IPlayerComponent>(component => component.Initialize(config, true));

    var viewTasks = from factory in viewFactories
                    where factory != null
                    select factory.CreateViews(config);
    var views = (await Task.WhenAll(viewTasks)).SelectMany(v => v);
    await task;

    // TODO(james7132): Move character view components to it's own factory
    ViewComponents = view.GetComponentsInChildren<IStateView<PlayerState>>().Concat(views).ToArray();
  }

  public void UpdateView(in PlayerState state) {
    if (ViewComponents == null) return;
    foreach (var component in ViewComponents) {
      component.UpdateView(state);
    }
  }

}

public interface IPlayerViewFactory {

  IEnumerable<IStateView<PlayerState>> CreatePlayerViews(PlayerConfig config);

}

}
