using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo.Players {

/// <summary>
/// A UI Component that allows displaying a players current damage.
/// </summary>
public class PlayerDamage : PlayerComponent {

  public TMP_Text DisplayText;
  public string Format;
  public Gradient DisplayColor;
  public float MinDamage;
  public float MaxDamage;

  float? lastDamage;

  public override void UpdateView(in PlayerState state) {
    if (DisplayText == null) {
      Debug.LogWarning($"{name} has a PlayerDamage without a Text display.");
      return;
    }
    SetDamage(state.Damage);
  }

  void SetDamage(float damage) {
    if (damage == lastDamage) return;
    var displayDamage = Mathf.Round(damage);
    if (string.IsNullOrEmpty(Format)) {
      DisplayText.text = displayDamage.ToString();
    } else {
      DisplayText.text = string.Format(Format, displayDamage);
    }
    DisplayText.color = GetColor(damage);
    lastDamage = damage;
  }

  Color GetColor(float damage) {
    var interp = Mathf.InverseLerp(MinDamage, MaxDamage, damage);
    interp = Mathf.Clamp01(interp);
    return DisplayColor.Evaluate(interp);
  }

}

}
