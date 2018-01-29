using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Players {
    
public class PlayerActive : MonoBehaviour, IStateView<PlayerState>, IPlayerSimulation {

  public Object[] TargetObjects;
  public bool Invert;

  public Task Initialize(PlayerConfig config, bool isView = false) => Task.CompletedTask;

  public void Presimulate(PlayerState state) => ApplyState(state);

  public PlayerState ResetState(PlayerState state) => state;

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) => state;

  public void ApplyState(PlayerState state) {
    var isActive = state.IsActive;
    if (Invert) isActive = !isActive;
    foreach (var target in TargetObjects) {
      ApplyActive(target, isActive);
    }
  }

  void ApplyActive(Object obj, bool active) {
    var component = obj as Behaviour;
    var gameObj = obj as GameObject;
    if (gameObj != null && gameObj.activeSelf != active) {
      gameObj.SetActive(active);
    }
    if (component != null) {
      component.enabled = active;
    }
  }

}

}

