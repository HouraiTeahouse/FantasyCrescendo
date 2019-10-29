using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[Serializable]
public class CharacterRespawn : CharacterComponent {

  public Vector3 Offset;
  GameObject platform;

  public override Task Init(Character character) {
    if (!character.IsView) return Task.CompletedTask;
    var prefab = Config.Get<VisualConfig>().RespawnPlatformPrefab;
    if (prefab == null) return Task.CompletedTask;
    platform = Object.Instantiate(prefab);
    platform.name = prefab.name;
    platform.transform.parent = character.transform;
    platform.transform.localPosition = Offset;
    return Task.CompletedTask;
  }

  public override void UpdateView(in PlayerState state) {
    if (platform == null) return;
    platform.SetActive(state.RespawnTimeRemaining > 0);
  }

}

}
