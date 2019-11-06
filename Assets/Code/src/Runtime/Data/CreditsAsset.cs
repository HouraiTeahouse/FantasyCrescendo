using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace HouraiTeahouse {

/// <summary>
/// Custom asset for allowing easy editing of the game's credits.!
/// Used with <see cref="CreditsUIBuilder"/> to automatically generate the UI on the credits screen.
/// </summary>
[CreateAssetMenu(menuName = "Fantasy Crescendo/Credits Asset")]
public class CreditsAsset : ScriptableObject {

  [Serializable]
  public class Category {
    public LocalizedString Name;
    public string[] Contributors;

    public override string ToString() {
      if (Contributors?.Length == 1) {
        return $"{Name.ToString()}: {Contributors[0]}";
      }
      return Name.ToString();
    }
  }

  public Category[] Categories;

}

}
