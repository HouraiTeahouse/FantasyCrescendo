using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterRespawn : MonoBehaviour, IPlayerView, IPlayerSimulation {

  public Vector3 Offset;

  GameObject platform;

  public Task Initialize(PlayerConfig config, bool isView = false) {
    if (isView) {
      var prefab = Config.Get<VisualConfig>().RespawnPlatformPrefab;
      if (prefab != null) {
        platform = Instantiate(prefab);
        platform.name = prefab.name;
        platform.transform.parent = transform;
        platform.transform.localPosition = Offset;
      }
    }
    return Task.CompletedTask;
  }

  public void Presimulate(PlayerState state) {
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    if (state.RespawnTimeRemaining > 0) {
      state.RespawnTimeRemaining--;
    } else {
      state.RespawnTimeRemaining = 0;
    }
    return state;
  }

  public PlayerState ResetState(PlayerState state) {
    state.Damage = 0f;
    state.ShieldDamage = 0;
    return state;
  }

  public void ApplyState(PlayerState state) {
    if (platform == null) return;
    platform.SetActive(state.RespawnTimeRemaining > 0);
  }

}

}
