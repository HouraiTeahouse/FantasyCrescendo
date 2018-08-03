using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public static class StateExtensions {

  public static State AddTransition(this State state, Func<State> transition) {
    Argument.NotNull(state);
    Argument.NotNull(transition);
    return state.AddTransition(ctx => transition());
  }

  public static State AddTransition(this State src, State dst, Func<CharacterContext, bool> predicate) {
    Argument.NotNull(src);
    Argument.NotNull(dst);
    Argument.NotNull(predicate);
    src.AddTransition(context => (predicate(context)) ? dst : null);
    return src;
  }

  public static IEnumerable<State> AddTransitions(this IEnumerable<State> states, 
                                                State.Transition transition) {
    foreach (var state in Argument.NotNull(states).Distinct()) {
      if (state != null)
        state.AddTransition(transition);
    }
    return states;
  }

  public static IEnumerable<State> AddTransitions(this IEnumerable<State> srcs, State dst,
                                                  Func<CharacterContext, bool> predicate) {
    return srcs.AddTransitions(context => (predicate(context)) ? dst :  null);
  }

}

}

