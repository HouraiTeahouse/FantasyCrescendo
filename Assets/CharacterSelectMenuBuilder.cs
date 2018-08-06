using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterSelectMenuBuilder : MonoBehaviour {

  public RectTransform Container;
  public RectTransform CharacterDisplayPrefab;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() => await BuildCharacterDisplays();

  async Task BuildCharacterDisplays() {
    if (CharacterDisplayPrefab == null || Container == null) return;
    await DataLoader.LoadTask.Task;
    IEnumerable<CharacterData> characters = Registry.Get<CharacterData>().Where(c => c.IsSelectable || c.IsVisible);
    if (!Debug.isDebugBuild) {
      characters = characters.Where(c => !c.IsDebug);
    }
    characters = characters.OrderBy(c => c.SourceGame).ThenBy(c => c.GameOrder);
    var tasks = new List<Task>();
    Debug.LogError(characters.Count());
    foreach (var character in characters) {
      var instance = Instantiate(CharacterDisplayPrefab);
      instance.SetParent(Container, false);
      instance.name = character.LongName;
      tasks.Add(instance.GetComponentInChildren<IInitializable<CharacterData>>().Initialize(character));
    }
    await Task.WhenAll(tasks);
  }

}

}