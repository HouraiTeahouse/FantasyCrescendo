using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A iniitalizer component that loads dynamically loadable data into
/// the global Registry.
/// </summary>
public class DataLoader : MonoBehaviour {

  /// <summary>
  /// The supported game mode types.
  /// </summary>
  public GameMode[] GameModes;

  /// <summary>
  /// Characters to load at startup.
  /// </summary>
  public CharacterData[] Characters;

  /// <summary>
  /// Scenes to load at startup.
  /// </summary>
  public SceneData[] Scenes;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    LoadAll("game mode", GameModes);
    LoadAll("character", Characters);
    LoadAll("scene", Scenes);
  }

  void LoadAll<T>(string type, IEnumerable<T> data) where T : Object, IIdentifiable {
    var registry = Registry.Get<T>();
    foreach (var datum in data) {
      registry.Add(datum);
      Debug.Log($"Registered {type}: {datum.name} ({datum.Id})");
    }
  }

}

}
