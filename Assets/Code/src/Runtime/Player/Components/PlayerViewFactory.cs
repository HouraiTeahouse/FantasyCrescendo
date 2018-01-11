using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// 
/// </summary>
public abstract class AbstractPlayerViewFactory : MonoBehaviour, IPlayerViewFactory {

  public abstract IEnumerable<IStateView<PlayerState>> CreatePlayerViews(PlayerConfig config);

}

public class PlayerViewFactory : AbstractPlayerViewFactory {

  public GameObject Prefab;

  public override IEnumerable<IStateView<PlayerState>> CreatePlayerViews(PlayerConfig config) {
    var view = Instantiate(Prefab);
    view.name = $"{Prefab.name} ({config.PlayerID + 1})";
    foreach (var initializer in view.GetComponentsInChildren<IInitializable<PlayerConfig>>()) {
      initializer.Initialize(config);
    }
    OnCreateView(config, view);
    return view.GetComponentsInChildren<IStateView<PlayerState>>();
  }

  public virtual void OnCreateView(PlayerConfig config, GameObject view) {
  }

}

}
