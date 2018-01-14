using HouraiTeahouse.Loadables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object representing a playable character.
/// </summary>
[CreateAssetMenu(menuName = "Fantasy Crescendo/Character")]
public class CharacterData : GameDataBase {

  ReadOnlyCollection<IAsset<Sprite>> _portraitsAssets;

  public string ShortName;
  public string LongName;

  [SerializeField, Resource(typeof(GameObject))] string _prefab;
  [SerializeField, Resource(typeof(Sprite))] string _icon;
  [SerializeField, Resource(typeof(Sprite))] string[] _portraits;
  [SerializeField, Resource(typeof(SceneData))] string _homeStage;
  [SerializeField, Resource(typeof(AudioClip))] string _victoryTheme;

  public IAsset<GameObject> Prefab => Asset.Get<GameObject>(_prefab);
  public IAsset<Sprite> Icon => Asset.Get<Sprite>(_icon);
  public IAsset<SceneData> HomeStage => Asset.Get<SceneData>(_homeStage);
  public IAsset<AudioClip> VictoryTheme => Asset.Get<AudioClip>(_victoryTheme);

  public ReadOnlyCollection<IAsset<Sprite>> Portraits {
    get {
      if (_portraitsAssets == null) {
        var portraitAssets = _portraits.Select(path => Asset.Get<Sprite>(path)).ToArray();
        _portraitsAssets = new ReadOnlyCollection<IAsset<Sprite>>(portraitAssets);
      }
      return _portraitsAssets;
    }
  }

  public void Unload() {
    new ILoadable[] { Prefab, Icon, HomeStage, VictoryTheme }
      .Concat(Portraits)
      .UnloadAll();
  }

  public override string ToString() => $"Character ({name})";

}

}
