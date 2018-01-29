using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters.UI {

public abstract class CharacterUIBase : MonoBehaviour, IInitializable<PlayerConfig>, 
                                        IInitializable<CharacterData> {

  public CharacterData Character;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    if (Character != null) {
      await Initialize(Character);
    }
  }

  public async Task Initialize(PlayerConfig config) {
    var character = Registry.Get<CharacterData>().Get(config.Selection.CharacterID);
    if (character == null) return;
    await Initialize(character);
  }

  public abstract Task Initialize(CharacterData character);

}

}