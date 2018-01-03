using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class DataManager : MonoBehaviour {

  public CharacterData[] Characters;

  void Awake() {
    var registry = Registry.Get<CharacterData>();
    foreach (var character in Characters) {
      registry.Add(character);
      Debug.LogFormat("Registered character: {0}", character.name);
    }
  }

}

}
