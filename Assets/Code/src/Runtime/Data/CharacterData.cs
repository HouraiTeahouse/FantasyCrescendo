using HouraiTeahouse.Loadables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public class CharacterPallete {

  [SerializeField, Resource(typeof(Sprite))] string _portrait;
  [SerializeField, Resource(typeof(GameObject))] string _prefab;

  public IAsset<Sprite> Portrait => Asset.Get<Sprite>(_portrait);
  public IAsset<GameObject> Prefab => Asset.Get<GameObject>(_prefab);

  public void Unload() {
    Portrait.Unload();
    Prefab.Unload();
  }

}

/// <summary>
/// A data object representing a playable character.
/// </summary>
[CreateAssetMenu(menuName = "Fantasy Crescendo/Character")]
public class CharacterData : GameDataBase {

  ReadOnlyCollection<IAsset<Sprite>> _portraitsAssets;

  public string ShortName;
  public string LongName;

  [SerializeField, Resource(typeof(SceneData))] string _homeStage;
  [SerializeField, Resource(typeof(AudioClip))] string _victoryTheme;
  [Header("Visuals")]
  [SerializeField, Resource(typeof(Sprite))] string _icon;
  [SerializeField] CharacterPallete[] _palletes;
  public Vector2 PortraitCropCenter;
  public float PortraitCropSize;

  public Rect PortraitCropRect {
    get {
      var size = Vector2.one * PortraitCropSize;
      var extents = size / 2;
      return new Rect(PortraitCropCenter - extents, size);
    }
  }

  public IAsset<Sprite> Icon => Asset.Get<Sprite>(_icon);
  public IAsset<SceneData> HomeStage => Asset.Get<SceneData>(_homeStage);
  public IAsset<AudioClip> VictoryTheme => Asset.Get<AudioClip>(_victoryTheme);

  public int PalleteCount => _palletes.Length;
  public CharacterPallete GetPallete(int index) => _palletes[index % PalleteCount];

  public void Unload() {
    new ILoadable[] { Icon, HomeStage, VictoryTheme }.UnloadAll();
    foreach (var pallete in _palletes) {
      pallete.Unload();
    }
  }

  public override string ToString() => $"Character ({name})";

}

}
