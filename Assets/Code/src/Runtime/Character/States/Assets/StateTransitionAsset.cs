using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public struct StateTransitionCondition {

  public Func<CharacterContext, bool> BuildPredicate() {
    throw new NotImplementedException();
  }
}

public class StateTransitionAsset : ScriptableObject {

  public StateAsset SourceState;
  public StateAsset DestinationState;
  public List<StateTransitionCondition> Conditions;
  public bool Muted;

  public State.Transition BuildTransition(State targetState) {
    Func<CharacterContext, bool>[] predicates = BuildPredicates(Conditions);
    return (context) => {
      if (Muted) return null;
      foreach (var predicate in predicates) {
        if (!predicate(context)) return null;
      }
      return targetState;
    };
  }

  static Func<CharacterContext, bool>[] BuildPredicates(IEnumerable<StateTransitionCondition> conditions) {
    return conditions.Select(cond => cond.BuildPredicate()).Where(cond => cond != null).ToArray();
  }

  /// <summary>
  /// Checks if the transition is related to a given state.
  /// </summary>
  /// <param name="state">the state to check against</param>
  /// <returns>true if the state is involved in the transition, false otherwise.</returns>
  public bool Involves(StateAsset state) => SourceState == state || DestinationState == state;

  internal static StateTransitionAsset Create(StateAsset src, StateAsset dst) {
    Argument.NotNull(src);
    Argument.NotNull(dst);
    var transition = ScriptableObject.CreateInstance<StateTransitionAsset>();
    transition.SourceState = src;
    transition.SourceState = dst;
    transition.Conditions = new List<StateTransitionCondition>();
    return transition;
  }

}

}
