using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public class CharacterPallete {

  public SpriteReference Portrait;
  public GameObjectReference Prefab;

  public void Unload() {
    Portrait.ReleaseAsset();
    Prefab.ReleaseAsset();
  }

}

/// <summary>
/// A data object representing a playable character.
/// </summary>
[CreateAssetMenu(menuName = "Fantasy Crescendo/Character")]
public class CharacterData : GameDataBase {

  public string ShortName;
  public string LongName;

  public SceneDataReference HomeStage;
  public AudioClipReference VictoryTheme;
  [Header("Visuals")]
  public SpriteReference Icon;
  public CharacterPallete[] Palletes;
  public Vector2 PortraitCropCenter;
  public float PortraitCropSize;

  public Rect PortraitCropRect {
    get {
      var size = Vector2.one * PortraitCropSize;
      var extents = size / 2;
      return new Rect(PortraitCropCenter - extents, size);
    }
  }

  public void Unload() {
    Icon.ReleaseAsset();
    HomeStage.ReleaseAsset();
    VictoryTheme.ReleaseAsset();
  }

  public override string ToString() => $"Character ({name})";

}

}
