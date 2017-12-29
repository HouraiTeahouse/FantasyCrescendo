using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Visual Config")]
public class VisualConfig : ScriptableObject {

  public Color[] PlayerColors = new[] {
    Color.red, Color.blue, Color.yellow, Color.green
  };

  public Color GetPlayerColor(uint playerId) {
    if (PlayerColors == null) return Color.grey;
    return PlayerColors[playerId % PlayerColors.Length];
  }

}

}
