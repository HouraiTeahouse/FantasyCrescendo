using HouraiTeahouse.EditorAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateAsset : ScriptableObject {

  [Type(typeof(State)), SerializeField]
  string _stateType;

  public CharacterStateData StateData;

  public State BuildState(uint id) {
    var state = CreateState(_stateType);
    state.Initalize(name, id, StateData);
    return state;
  }

  static State CreateState(string type) {
    try {
      Type stateType = Type.GetType(type);
      return Activator.CreateInstance(stateType) as State;
    } catch {
      return new State();
    }
  }

  public static StateAsset Create(string name = null) {
    var state = ScriptableObject.CreateInstance<StateAsset>();
    state.name = name ?? "State";
    return state;
  }

}

}