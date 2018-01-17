using HouraiTeahouse.EditorAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class RegisteredBehaviour<T, TID> : MonoBehaviour, IEntity where T : RegisteredBehaviour<T, TID> {

  [SerializeField, ReadOnly] TID _id;

  public TID Id {
    get { return _id; }
    protected set { _id = value; }
  }
  uint IEntity.Id => Convert.ToUInt32(_id);

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  protected virtual void Awake() => Registry.Get<T>().Add((T)this);

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  protected virtual void OnDestroy() => Registry.Get<T>().Remove((T)this);

}

}