using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerStateDebugView : MonoBehaviour, IPlayerView {

  public PlayerConfig Config;
  public PlayerState State;

#if !UNITY_EDITOR
  void Awake() => DestroyImmediate(this);
#endif

  public Task Initialize(PlayerConfig config, bool isView = false) {
    Config = config;
    return Task.CompletedTask;
  }

  public void ApplyState(in PlayerState state) => State = state;

}
    
}
