using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterRespawn : MonoBehaviour, IPlayerView, IPlayerSimulation {

  public Vector3 Offset;

  float defaultDamage;
  GameObject platform;

  public Task Initialize(PlayerConfig config, bool isView = false) {
    defaultDamage = config.DefaultDamage;
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

  public void Presimulate(in PlayerState state) {}

  public void Simulate(ref PlayerState state, PlayerInputContext input) {
    if (state.RespawnTimeRemaining > 0) {
      state.RespawnTimeRemaining--;
    } else {
      state.RespawnTimeRemaining = 0;
    }
  }

  public void ResetState(ref PlayerState state) {
    state.Damage = defaultDamage;
    state.ShieldDamage = 0;
  }

  public void ApplyState(in PlayerState state) {
    if (platform == null) return;
    platform.SetActive(state.RespawnTimeRemaining > 0);
  }

}

}
