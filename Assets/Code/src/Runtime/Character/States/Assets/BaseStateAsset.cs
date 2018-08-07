using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public abstract class BaseStateAsset : ScriptableObject {

  public abstract IEnumerable<StateAsset> GetBaseStates();

  public static T Create<T>(string name = null) where T : BaseStateAsset {
    var state = ScriptableObject.CreateInstance<T>();
    state.name = name ?? "State";
    return state;
  }

}

}