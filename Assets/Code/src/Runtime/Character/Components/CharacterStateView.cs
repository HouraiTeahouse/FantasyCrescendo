using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterStateView : MonoBehaviour, IPlayerView {

  public PlayerConfig Config;
  public PlayerState State;

  public Task Initialize(PlayerConfig config, bool isView = false) {
    Config = config;
    return Task.CompletedTask;
  }

  public void ApplyState(PlayerState state) => State = state;

}
    
}
