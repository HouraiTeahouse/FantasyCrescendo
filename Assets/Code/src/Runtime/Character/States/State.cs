using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class State : IEntity, IStateView<PlayerState>,
                     ISimulation<PlayerState, PlayerInputContext> {

  public delegate State Transition(CharacterContext context);

  public string Name { get; private set; }
  public uint Id { get; private set; }
  public CharacterStateData Data { get; private set; }
  public string AnimatorName { get; private set; }
  public int AnimatorHash { get; private set; }

  internal void Initalize(string name, uint id, CharacterStateData data) {
    Name = name;
    Id = id;
    Data = Argument.NotNull(data);
    AnimatorName = Name.Replace(".", "-");
    AnimatorHash = Animator.StringToHash(AnimatorName);
  }

  public virtual Task Initalize(PlayerConfig config, GameObject gameObject, 
                                bool isView) => null;

  readonly List<Transition> _transitions;
  public ReadOnlyCollection<Transition> Transitions { get; }

  protected State() {
    _transitions = new List<Transition>();
    Transitions = new ReadOnlyCollection<Transition>(_transitions);
  }

  public State AddTransition(Transition transition) {
    Argument.NotNull(transition);
    _transitions.Add(transition);
    return this;
  }

  public virtual State Passthrough(CharacterContext context) => EvaluateTransitions(context);

  public State EvaluateTransitions(CharacterContext context) {
    foreach (var transition in _transitions) {
      var newState = transition(context);
      if (newState != null && newState?.Data?.EntryPolicy != StateEntryPolicy.Blocked) {
        return newState;
      }
    }
    return null;
  }

  public virtual StateEntryPolicy GetEntryPolicy(CharacterContext context) => StateEntryPolicy.Normal;

  public virtual void OnStateEnter(CharacterContext context) {
    context.State.StateTick = 0;
    context.State.ResetPlayersHit();
  }
  public virtual void OnStateUpdate(CharacterContext context) {}
  public virtual void OnStateExit(CharacterContext context) {}

  public void Simulate(ref PlayerState state, PlayerInputContext simulate) {}
  public void ApplyState(ref PlayerState state) {}

  public State AddTransitionTo(State state, 
                               Func<CharacterContext, bool> extraCheck = null) {
    if (extraCheck != null)
      AddTransition(ctx => ctx.NormalizedStateTime >= 1.0f && extraCheck(ctx) ? state : null);
    else
      AddTransition(ctx => ctx.NormalizedStateTime >= 1.0f ? state : null);
    return this;
  }

  public static bool operator ==(State lhs, State rhs) {
    if (object.ReferenceEquals(lhs, null) && object.ReferenceEquals(rhs, null))
      return true;
    if (object.ReferenceEquals(lhs, null) ^ object.ReferenceEquals(rhs, null))
      return false;
    return lhs.AnimatorHash == rhs.AnimatorHash;
  }

  public static bool operator !=(State lhs, State rhs) {
    return !(lhs == rhs);
  }

  public override bool Equals(object obj) {
    var state = obj as State;
    return object.ReferenceEquals(state, null) ? false : state == this;
  }

  public override string ToString() => $"CharacterState({Name})";

  public override int GetHashCode() => AnimatorHash;

}

public static class CharacterStateExtensions {

public static IEnumerable<State> AddTransitionTo(this IEnumerable<State> states,
                                                 State state) {
  foreach (State characterState in states) {
    characterState.AddTransition(ctx => ctx.NormalizedStateTime >= 1.0f ? state : null);
  }
  return states;
}

public static void Chain(this IEnumerable<State> states) {
  State last = null;
  foreach (State state in states) {
    if (state == null) continue;
    if (last != null) last.AddTransitionTo(state);
    last = state;
  }

}

}

}

