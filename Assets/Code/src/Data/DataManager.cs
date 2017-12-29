using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class DataManager : MonoBehaviour {

  public CharacterData[] Characters;
  public ScriptableObject[] Configs;

  void Awake() {
    foreach (var config in Configs) {
      Config.Register(config);
      Debug.LogFormat("Loaded configuration: {0}", config.GetType().Name);
    }

    var registry = Registry.Get<CharacterData>();
    foreach (var character in Characters) {
      registry.Add(character);
      Debug.LogFormat("Registered character: {0}", character.name);
    }
  }

}

}
