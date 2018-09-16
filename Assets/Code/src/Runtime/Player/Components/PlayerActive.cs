using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Players {
    
public class PlayerActive : MonoBehaviour, IStateView<PlayerState>, IPlayerSimulation {

  public Object[] TargetObjects;
  public bool Invert;

  public Task Initialize(PlayerConfig config, bool isView = false) => Task.CompletedTask;

  public void Presimulate(in PlayerState state) => UpdateView(state);

  public void ResetState(ref PlayerState state) {}

  public void Simulate(ref PlayerState state, PlayerInputContext input) {}

  public void UpdateView(in PlayerState state) {
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

