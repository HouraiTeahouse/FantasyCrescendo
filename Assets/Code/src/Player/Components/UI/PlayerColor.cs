using HouraiTeahouse.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

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
