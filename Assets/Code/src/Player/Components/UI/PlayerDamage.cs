using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A UI Component that allows displaying a players current damage.
/// </summary>
public class PlayerDamage : MonoBehaviour, IStateView<PlayerState> {

  public Text DisplayText;
  public string Format;

  public void ApplyState(PlayerState state) {
    if (DisplayText == null) {
      Debug.LogWarning($"{name} has a PlayerDamage without a Text display.");
      return;
    }
    var displayDamage = Mathf.Round(state.Damage);
    if (string.IsNullOrEmpty(Format)) {
      DisplayText.text = displayDamage.ToString();
    } else {
      DisplayText.text = string.Format(Format, displayDamage);
    }
  }

}

}
