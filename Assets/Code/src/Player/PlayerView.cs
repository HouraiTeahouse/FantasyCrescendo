using HouraiTeahouse.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerView : IInitializable<PlayerConfig>, IStateView<PlayerState> {

  GameObject View;

  IStateView<PlayerState>[] ViewComponents;

  public ITask Initialize(PlayerConfig config) {
    var selection = config.Selection;
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    View = Object.Instantiate(character.Prefab);
    View.name = string.Format("Player {0} View ({1}, {2})",
                              config.PlayerID + 1, character.name,
                              selection.Pallete);

    PlayerUtil.DestroyAll(View, typeof(Collider));

    var task = View.Broadcast<ICharacterComponent>(
        component => component.Initialize(config, true));

    ViewComponents = View.GetComponentsInChildren<IStateView<PlayerState>>();

    return task;
  }

  public void ApplyState(PlayerState state) {
    foreach (var component in ViewComponents) {
      component.ApplyState(state);
    }
  }

}

}
