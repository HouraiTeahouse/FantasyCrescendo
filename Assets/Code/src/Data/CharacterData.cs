using System;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object representing a playable character.
/// </summary>
[CreateAssetMenu(fileName = "New Chararacter", menuName = "Character")]
public class CharacterData : ScriptableObject, IIdentifiable {

  [SerializeField] uint _id;
  public string ShortName;
  public string LongName;
  public GameObject Prefab;

  public uint Id {
    get { return _id; }
  }

  void Reset() {
    var newId = new Random().Next();
    _id = (uint)newId + (uint)Int32.MaxValue;
  }

}

}
