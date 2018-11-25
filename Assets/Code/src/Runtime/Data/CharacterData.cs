using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public class CharacterPallete {

  [AssetReferenceTypeRestriction(typeof(Sprite))] public AssetReference Portrait;
  [AssetReferenceTypeRestriction(typeof(GameObject))] public AssetReference Prefab;

  public void Unload() {
    Portrait.ReleaseAsset<Sprite>();
    Prefab.ReleaseAsset<GameObject>();
  }

}

/// <summary>
/// A data object representing a playable character.
/// </summary>
[CreateAssetMenu(menuName = "Fantasy Crescendo/Character")]
public class CharacterData : GameDataBase {

  public string ShortName;
  public string LongName;

  [AssetReferenceTypeRestriction(typeof(SceneData))] public AssetReference HomeStage;
  [AssetReferenceTypeRestriction(typeof(AudioClip))] public AssetReference VictoryTheme;
  [Header("Visuals")]
  [AssetReferenceTypeRestriction(typeof(Sprite))] public AssetReference Icon;
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
    Icon.ReleaseAsset<Sprite>();
    HomeStage.ReleaseAsset<SceneData>();
    VictoryTheme.ReleaseAsset<AudioClip>();
  }

  public override string ToString() => $"Character ({name})";

}

}
