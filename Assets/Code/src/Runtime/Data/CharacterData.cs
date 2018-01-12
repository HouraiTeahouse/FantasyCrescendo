using HouraiTeahouse.Loadables;
using System;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object representing a playable character.
/// </summary>
[CreateAssetMenu(menuName = "Fantasy Crescendo/Character")]
public class CharacterData : GameDataBase {

  public string ShortName;
  public string LongName;

  [SerializeField, Resource(typeof(GameObject))] string _prefab;
  [SerializeField, Resource(typeof(Sprite))] string _icon;
  [SerializeField, Resource(typeof(Sprite))] string[] _portraits;

  public IAsset<GameObject> Prefab => Asset.Get<GameObject>(_prefab);
  public IAsset<Sprite> Icon => Asset.Get<Sprite>(_icon);

  public IAsset<Sprite> GetPortrait(int index) {
    return Asset.Get<Sprite>(_portraits[index % _portraits.Length]);
  }

}

}
