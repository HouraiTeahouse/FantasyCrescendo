using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class PlayerComponent : MonoBehaviour, IPlayerView, IPlayerSimulation {

  public virtual Task Initialize(PlayerConfig config, bool isView = false) {
    return Task.CompletedTask;
  }

  public virtual void Presimulate(in PlayerState state) => UpdateView(state);

  public virtual void Simulate(ref PlayerState state, PlayerInputContext input) {}

  public virtual void UpdateView(in PlayerState state) {}

  public virtual void Dispose() => ObjectUtil.Destroy(this);

  public virtual void ResetState(ref PlayerState state) {}

}

public class PlayerStateDebugView : PlayerComponent {

  public PlayerConfig Config;
  public PlayerState State;

#if !UNITY_EDITOR
  void Awake() => DestroyImmediate(this);
#endif

  public override Task Initialize(PlayerConfig config, bool isView = false) {
    Config = config;
    return base.Initialize(config, isView);
  }

  public override void UpdateView(in PlayerState state) => State = state;

}
    
}
