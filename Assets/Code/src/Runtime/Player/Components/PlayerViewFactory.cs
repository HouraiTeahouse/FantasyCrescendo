using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerViewFactory<T> : ViewFactory<T, PlayerConfig> {

  public GameObject Prefab;

  public override async Task<IStateView<T>[]> CreateViews(PlayerConfig config) {
    var view = Instantiate(Prefab);
    view.name = $"{Prefab.name} ({config.PlayerID + 1})";
    await view.Initialize(config);
    OnCreateView(config, view);
    return view.GetComponentsInChildren<IStateView<T>>();
  }

  public virtual void OnCreateView(PlayerConfig config, GameObject view) {
  }

}

}
