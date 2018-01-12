using System.Collections.Generic;
using HouraiTeahouse.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// Manages the visual display of a single player's state within a multiplayer match.
/// </summary>
public class PlayerView : IInitializable<PlayerConfig>, IStateView<PlayerState> {

  GameObject View;

  IStateView<PlayerState>[] ViewComponents;

  readonly IEnumerable<IPlayerViewFactory> viewFactories;

  public PlayerView(IEnumerable<IPlayerViewFactory> viewFactories) {
    this.viewFactories = viewFactories;
  }

  public ITask Initialize(PlayerConfig config) {
    var selection = config.Selection;
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    return character.Prefab.LoadAsync().Then(prefab => {
      View = Object.Instantiate(prefab);
      View.name = $"Player {config.PlayerID + 1} View ({character.name}, {selection.Pallete})";

      PlayerUtil.DestroyAll(View, typeof(Collider));

      var task = View.Broadcast<IPlayerComponent>(
          component => component.Initialize(config, true));

      var viewComponents = new List<IStateView<PlayerState>>();
      // TODO(james7132): Move character view components to it's own factory
      viewComponents.AddRange(View.GetComponentsInChildren<IStateView<PlayerState>>());
      foreach (var factory in viewFactories) {
        if (factory != null) {
          viewComponents.AddRange(factory.CreatePlayerViews(config));
        }
      }

      ViewComponents = viewComponents.ToArray();

      return task;
    });
  }

  public void ApplyState(PlayerState state) {
    if (ViewComponents == null) {
      return;
    }
    foreach (var component in ViewComponents) {
      component.ApplyState(state);
    }
  }

}

public interface IPlayerViewFactory {

  IEnumerable<IStateView<PlayerState>> CreatePlayerViews(PlayerConfig config);

}

}
