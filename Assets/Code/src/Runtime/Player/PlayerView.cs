using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Players {

/// <summary>
/// Manages the visual display of a single player's state within a multiplayer match.
/// </summary>
public class PlayerView : IInitializable<PlayerConfig>, IStateView<PlayerState> {

  GameObject View;

  IStateView<PlayerState>[] ViewComponents;

  readonly IEnumerable<IViewFactory<PlayerState, PlayerConfig>> viewFactories;

  public PlayerView(IEnumerable<IViewFactory<PlayerState, PlayerConfig>> viewFactories) {
    this.viewFactories = viewFactories;
  }

  public async Task Initialize(PlayerConfig config) {
    var selection = config.Selection;
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    var prefab = await character.Prefab.LoadAsync();

    View = Object.Instantiate(prefab);
    View.name = $"Player {config.PlayerID + 1} View ({character.name}, {selection.Pallete})";

    PlayerUtil.DestroyAll(View, typeof(Collider), typeof(Hurtbox), typeof(Hitbox));

    var task = View.Broadcast<IPlayerComponent>( component => component.Initialize(config, true));

    var viewTasks = from factory in viewFactories
                    where factory != null
                    select factory.CreateViews(config);
    var views = (await Task.WhenAll(viewTasks)).SelectMany(v => v);
    await task;

    // TODO(james7132): Move character view components to it's own factory
    ViewComponents = View.GetComponentsInChildren<IStateView<PlayerState>>().Concat(views).ToArray();
  }

  public void ApplyState(ref PlayerState state) {
    if (ViewComponents == null) {
      return;
    }
    foreach (var component in ViewComponents) {
      component.ApplyState(ref state);
    }
  }

}

public interface IPlayerViewFactory {

  IEnumerable<IStateView<PlayerState>> CreatePlayerViews(PlayerConfig config);

}

}
