using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateController<T, TContext> where T : State<TContext> {

  public T DefaultState { get; private set; }
  readonly HashSet<T> _states;
  public ReadOnlyCollection<T> States { get; private set; }

  public event Action<T, T> OnStateChange;

  internal StateController(StateControllerBuilder<T, TContext> builder) {
    DefaultState = builder.DefaultState;
    _states = builder._states;
    States = new ReadOnlyCollection<T>(builder.States.ToArray());
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
  public void ChangeState(T src, T dst) => ChangeState(src, dst, default(TContext));

  void ChangeState(T src, T dst, TContext context) {
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
  public T UpdateState(T src, TContext context) => UpdateState(src, context, false);

  T UpdateState(T src, TContext context, bool passthrough) {
    T dst;
    if (passthrough) {
      dst = src.Passthrough(context) as T;
    } else {
      src?.OnStateUpdate(context);
      dst = src.EvaluateTransitions(context) as T;
    }
    if (dst == null) { 
      return src;
    } else {
      switch (dst.GetEntryPolicy(context)) {
        case StateEntryPolicy.Normal:
          ChangeState(src, dst, context);
          break;
        case StateEntryPolicy.Passthrough:
          ChangeState(src, dst, context);
          dst = UpdateState(dst, context, true);
          break;
        case StateEntryPolicy.Blocked:
          break;
      }
      return dst;
    } 
  }

}

}

