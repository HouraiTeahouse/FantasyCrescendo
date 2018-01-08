using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A iniitalizer component that loads dynamically loadable data into
/// the global Registry.
/// </summary>
public class DataLoader : MonoBehaviour {

  /// <summary>
  /// Characters to load at startup.
  /// </summary>
  public CharacterData[] Characters;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    var registry = Registry.Get<CharacterData>();
    foreach (var character in Characters) {
      registry.Add(character);
      Debug.LogFormat("Registered character: {0}", character.name);
    }
  }

}

}
