using System;
using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[Serializable]
public class StateTransitionCondition {

  public enum ConditionType {
    Time,
    InputButtonWasPressed, 
    InputButtonWasReleased, 
    InputButtonHeld, 
    InputDirection,
    PlayerDirection,
    IsGrounded,
    IsHit,
    IsGrabbingLedge,
    IsRespawing,
    CanJump,
    IsShieldBroken
  }

  public enum ButtonType {
    Attack, Special, Jump, Shield
  }
  
  public enum AxisType {
    Movement, Smash
  }

  public ConditionType Type;
  public ButtonType Button;
  public AxisType Axis;
  public Direction AxisDirection;

  public float FloatParam = 1.0f;
  public bool BoolParam;

  public Func<CharacterContext, bool> BuildPredicate() {
    switch (Type)  {
      case ConditionType.InputButtonWasPressed: 
      case ConditionType.InputButtonWasReleased: 
      case ConditionType.InputButtonHeld:
        return ButtonPredicate();
      case ConditionType.InputDirection: 
        return AxisPredicate();
      case ConditionType.PlayerDirection:
        return ctx => (ctx.Direction > 0.0f) == BoolParam;
      case ConditionType.Time:
        return ctx => ctx.NormalizedStateTime >= FloatParam;
      case ConditionType.IsGrounded:
        return ctx => ctx.IsGrounded == BoolParam;
      case ConditionType.IsHit:
        return ctx => ctx.State.IsHit == BoolParam;
      case ConditionType.IsGrabbingLedge:
        return ctx => ctx.State.IsGrabbingLedge == BoolParam;
      case ConditionType.CanJump:
        return ctx => ctx.CanJump == BoolParam;
      case ConditionType.IsShieldBroken:
        return ctx => ctx.ShieldBroken == BoolParam;
      default: return null;
    }
  }

  Func<CharacterContext, bool> ButtonPredicate() {
    Func<CharacterContext, ButtonContext> button;
    switch (Button) {
      case ButtonType.Attack: button = ctx => ctx.Input.Attack; break;
      case ButtonType.Special: button = ctx => ctx.Input.Special; break;
      case ButtonType.Shield: button = ctx => ctx.Input.Shield; break;
      case ButtonType.Jump: button = ctx => ctx.Input.Jump; break;
      default: throw new InvalidOperationException();
    }
    switch (Type) {
      case ConditionType.InputButtonWasPressed: return ctx => button(ctx).WasPressed;
      case ConditionType.InputButtonWasReleased: return ctx => button(ctx).WasReleased;
      case ConditionType.InputButtonHeld: return ctx => button(ctx).Current;
      default: throw new InvalidOperationException();
    }
  }

  Func<CharacterContext, bool> AxisPredicate() {
    Func<CharacterContext, DirectionalInput> axis;
    switch (Axis) {
      case AxisType.Movement: axis = ctx => ctx.Input.Movement; break;
      case AxisType.Smash: axis = ctx => ctx.Input.Smash; break;
      default: return null;
    }
    return ctx => axis(ctx).Direction == AxisDirection;
  }

}

}
