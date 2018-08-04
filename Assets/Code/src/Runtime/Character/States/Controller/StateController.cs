using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateController {

  public State DefaultState { get; private set; }
  public ReadOnlyCollection<State> States { get; private set; }

  public event Action<State, State> OnStateChange;

  internal StateController(StateControllerBuilder builder) {
    DefaultState = builder.DefaultState;
    States = new ReadOnlyCollection<State>(builder.States.ToArray());
  }

  /// <summary>
  /// Changes the current state of the state controller to the provided state.
  /// This is suggested to be used for what is normally seen as a state transition.
  /// This will invoke the <see cref="OnStateChange"/> event.
  /// For a version that doesn't invoke the event, use <see cref="SetState"/>
  /// </summary>
  /// <param name="dst">the state to change to</param>
  /// <exception cref="System.ArgumentException"><paramref cref="state">/ is not a state of the StateController</exception>
  /// <exception cref="System.ArgumentNullException"><paramref cref="state"/> is null</exception>
  public void ChangeState(State src, State dst) => ChangeState(src, dst, null);

  void ChangeState(State src, State dst, CharacterContext context) {
    if (src == dst) return;
    if (context != null) {
      src?.OnStateExit(context);
      dst?.OnStateEnter(context);
    }
    OnStateChange?.Invoke(src, dst);
  }

  /// <summary>
  /// Evaluates possible state transitions, and updates <see cref="CurrentState"/> approriately.
  /// </summary>
  /// <param name="context">the context object for evaluating the chnage against</param>
  /// <returns>the new current state</returns>
  public State UpdateState(State src, CharacterContext context) {
    State dst;
    src?.OnStateUpdate(context);
    dst = src.EvaluateTransitions(context);
    if (dst == null) { 
      return src;
    } else {
      switch (dst.GetEntryPolicy(context)) {
        case StateEntryPolicy.Normal:
        case StateEntryPolicy.Passthrough:
          ChangeState(src, dst, context);
          break;
        case StateEntryPolicy.Blocked:
          break;
      }
      return dst;
    } 
  }

}

}

