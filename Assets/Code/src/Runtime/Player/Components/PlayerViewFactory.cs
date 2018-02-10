using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerViewFactory<T> : ViewFactory<T, PlayerConfig> {

  public GameObject Prefab;

  public override async Task<IStateView<T>[]> CreateViews(PlayerConfig config) {
    var view = Instantiate(Prefab);
    view.name = $"{Prefab.name} ({config.PlayerID + 1})";
    Preinitialize(config, view);
    await view.Initialize(config);
    Postinitalize(config, view);
    return view.GetComponentsInChildren<IStateView<T>>();
  }

  protected virtual void Preinitialize(PlayerConfig config, GameObject view) { }
  protected virtual void Postinitalize(PlayerConfig config, GameObject view) { }

}

}
