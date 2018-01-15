using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerViewFactory : AbstractViewFactory<PlayerState, PlayerConfig> {

  public GameObject Prefab;

  public override async Task<IStateView<PlayerState>[]> CreateViews(PlayerConfig config) {
    var view = Instantiate(Prefab);
    view.name = $"{Prefab.name} ({config.PlayerID + 1})";
    var taskList = new List<Task>();
    foreach (var initializer in view.GetComponentsInChildren<IInitializable<PlayerConfig>>()) {
      taskList.Add(initializer.Initialize(config));
    }
    await Task.WhenAll(taskList);
    OnCreateView(config, view);
    return view.GetComponentsInChildren<IStateView<PlayerState>>();
  }

  public virtual void OnCreateView(PlayerConfig config, GameObject view) {
  }

}

}
