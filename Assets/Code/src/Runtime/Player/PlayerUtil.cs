using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo.Players {

/// <summary>
/// Utility static class for management of Player related components.
/// </summary>
public static class PlayerUtil {

  static Type[] kBannedViewComponents = new[] {
    typeof(Collider), typeof(Hurtbox), typeof(Hitbox)
  };
  static Type[] kBannedSimulationComponents = new[] {
    typeof(Renderer), typeof(MeshFilter)
  };

  public static async Task<GameObject> Instantiate(PlayerConfig config, bool isView = false) {
    var selection = config.Selection;
    var obj = await selection.GetPallete().Prefab.InstantiateAsync();
    var type = isView ? "View" : "Simulation";
    obj.name = $"Player {config.PlayerID + 1} {type} ({selection.GetPrettyString()})";

    var bannedComponents = isView ? kBannedViewComponents : kBannedSimulationComponents;
    foreach (var componentType in bannedComponents) {
        foreach (var component in ObjectUtility.GetAll(obj, componentType)) {
            Object.DestroyImmediate(component);
        }
    }
    return obj;
  }

  public static PlayerRespawnEvent RespawnPlayer(PlayerDiedEvent evt) {
    if (evt.Respawned) return null;
    var respawnEvent = new PlayerRespawnEvent(evt);
    Mediator.Global.Publish(respawnEvent);
    evt.Copy(respawnEvent);
    evt.Respawned = true;
    return respawnEvent;
  }

}

}
