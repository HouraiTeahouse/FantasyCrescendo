using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterState : State<CharacterContext>, IEntity, IStateView<PlayerState>,
                              ISimulation<PlayerState, PlayerInputContext> {

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

  public override void OnStateEnter(CharacterContext context) {
    context.State.StateTick = 0;
    context.State.ResetPlayersHit();
  }

  public void Simulate(ref PlayerState state, PlayerInputContext simulate) {}

  public void ApplyState(ref PlayerState state) {}

  public CharacterState AddTransitionTo(CharacterState state, 
                                        Func<CharacterContext, bool> extraCheck = null) {
    if (extraCheck != null)
      AddTransition(ctx => ctx.NormalizedStateTime >= 1.0f && extraCheck(ctx) ? state : null);
    else
      AddTransition(ctx => ctx.NormalizedStateTime >= 1.0f ? state : null);
    return this;
  }

  public override State<CharacterContext> Passthrough(CharacterContext context) {
    var altContext = context.Clone();
    altContext.State.StateTick = uint.MaxValue;
    return EvaluateTransitions(altContext);
  }

  public override StateEntryPolicy GetEntryPolicy (CharacterContext context) {
    return Data.EntryPolicy;
  }

  public static bool operator ==(CharacterState lhs, CharacterState rhs) {
    if (object.ReferenceEquals(lhs, null) && object.ReferenceEquals(rhs, null))
      return true;
    if (object.ReferenceEquals(lhs, null) ^ object.ReferenceEquals(rhs, null))
      return false;
    return lhs.AnimatorHash == rhs.AnimatorHash;
  }

  public static bool operator !=(CharacterState lhs, CharacterState rhs) {
    return !(lhs == rhs);
  }

  public override bool Equals(object obj) {
    var state = obj as CharacterState;
    return object.ReferenceEquals(state, null) ? false : state == this;
  }

  public override string ToString() => $"CharacterState({Name})";

  public override int GetHashCode() => AnimatorHash;

}

public static class CharacterStateExtensions {

public static IEnumerable<CharacterState> AddTransitionTo(this IEnumerable<CharacterState> states,
                                                          State<CharacterContext> state) {
  foreach (CharacterState characterState in states) {
    characterState.AddTransition(ctx => ctx.NormalizedStateTime >= 1.0f ? state : null);
  }
  return states;
}

public static void Chain(this IEnumerable<CharacterState> states) {
  CharacterState last = null;
  foreach (CharacterState state in states) {
    if (state == null) continue;
    if (last != null) last.AddTransitionTo(state);
    last = state;
  }

}

}

}

