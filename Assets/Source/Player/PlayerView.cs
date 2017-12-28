using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerView : IStateView<PlayerState> {

  GameObject View;

  IStateView<PlayerState>[] ViewComponents;

  public PlayerView(PlayerConfig config) {
    var character = Registry.Get<CharacterData>().Get(config.Selection.CharacterID);
    View = Object.Instantiate(character.Prefab);
    View.name = character.name + " (Player View)";

    PlayerUtil.DestroyAll(View, typeof(Collider));

    foreach (var component in View.GetComponentsInChildren<ICharacterComponent>()) {
      component.Initialize(config);
    }

    ViewComponents = View.GetComponentsInChildren<IStateView<PlayerState>>();
  }

  public void ApplyState(PlayerState state) {
    foreach (var component in ViewComponents) {
      component.ApplyState(state);
    }
  }

}

}
