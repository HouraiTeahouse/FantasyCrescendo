using HouraiTeahouse.EditorAttributes;
using System;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class GameDataBase : ScriptableObject, IIdentifiable {

  [SerializeField, ReadOnly] uint _id;

  public uint Id => _id;

  public bool IsSelectable = true;
  public bool IsVisible = true;
  public bool IsDebug = false;

  void Reset() => RegenerateID();

  [ContextMenu("Regenerate ID")]
  void RegenerateID() {
    var newId = new Random().Next();
    _id = (uint)newId + (uint)Int32.MaxValue;
  }

}

}
