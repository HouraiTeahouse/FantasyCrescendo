using HouraiTeahouse.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A UI component for changing a Unity UI component's color to match that
/// of a given player. The color is defined by the registered VisualConfig.
/// </summary>
public class PlayerColor : MonoBehaviour, IInitializable<PlayerConfig> {

  public Graphic Graphic;

  public ITask Initialize(PlayerConfig config) {
    if (Graphic != null) {
      Graphic.color = Config.Get<VisualConfig>().GetPlayerColor(config.PlayerID);
    } else {
      Debug.LogWarning($"{name} has a PlayerColor without a Graphic display.");
    }
    return Task.Resolved;
  }

}

}
