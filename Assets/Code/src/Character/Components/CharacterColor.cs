using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterColor : MonoBehaviour, ICharacterComponent {

  public void Initialize(PlayerConfig config) {
    //TODO(james7132): Load materials properly here.
    var color = Config.Get<VisualConfig>().GetPlayerColor(config.PlayerID);
    foreach (var renderer in GetComponentsInChildren<Renderer>()) {
      renderer.material.color = color;
    }
  }

}

}
