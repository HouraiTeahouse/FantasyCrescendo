using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[RequireComponent(typeof(CharacterPhysics))]
public class CharacterMovement : MonoBehaviour, IPlayerSimulation {

  public CharacterPhysics Physics;
  public CharacterStateMachine StateMachine;

  public float[] JumpPower;

  public int MaxJumpCount =>  JumpPower?.Length ?? 0;

  CharacterMover[] Movers;

  public Task Initialize(PlayerConfig config, bool isView) {
    if (Physics == null) {
      Physics = GetComponent<CharacterPhysics>();
    }

    Movers = new CharacterMover[] {
      new HitstunMovement(),
      new RespawnMovement(),
      new LedgeMovement(),
      new AerialMovement(),
      new GroundMovement(),
    };

    foreach (var mover in Movers) {
      mover.Character = this;
    }

    return Task.CompletedTask;
  }
  
  public PlayerState ResetState(PlayerState state) {
    state.IsFastFalling = false;
    return state;
  }

  public void Presimulate(PlayerState state) { }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    foreach (var mover in Movers) {
      if (mover.ShouldMove(state)) {
        return mover.Move(state, input);
      }
    }
    return state;
  }

  public bool CanJump(PlayerState state) => state.JumpCount < MaxJumpCount;
  public float GetJumpPower(PlayerState state) => JumpPower[state.JumpCount];

  public void Jump(ref PlayerState state) {
    if (CanJump(state)) {
      state.VelocityY = GetJumpPower(state);
      state.IsFastFalling = false;
      state.JumpCount++;
    }
  }

  public void ApplyControlledMovement(ref PlayerState state, Vector2 movementInput) {
    var data = StateMachine.GetControllerState(state).Data;
    ApplyDirection(ref state, movementInput, data);
    ApplyMovement(ref state, movementInput, data);
  }

  void ApplyDirection(ref PlayerState state, Vector2 movement, CharacterStateData data) {
    switch (data.DirectionMode) {
      case DirectionMode.PlayerControlled:
        if (movement.x > DirectionalInput.DeadZone) {
          state.Direction = true;
        } else if (movement.x < -DirectionalInput.DeadZone) {
          state.Direction = false;
        }
        break;
      case DirectionMode.AlwaysLeft: 
        state.Direction = false; 
        break;
      case DirectionMode.AlwaysRight: 
        state.Direction = true; 
        break;
      default: 
        break;
    }
  }

  void ApplyMovement(ref PlayerState state, Vector2 movement, CharacterStateData data) {
    float dir = state.Direction ? 1f : -1f;
    float speed = 0f;
    switch (data.MovementType) {
      case MovementType.Normal:
        speed = data.GetScaledMoveSpeed(movement);
        break;
      case MovementType.DirectionalInfluenceOnly:
        dir = movement.x > 0 ? 1f : -1f;
        speed = data.GetScaledMoveSpeed(movement);
        break;
      case MovementType.Locked:
        speed = 0;
        break;
      case MovementType.Forced:
        speed = data.MaxMoveSpeed;
        break;
    }
    state.VelocityX = dir * speed;
  }


}

public abstract class CharacterMover {

  public CharacterMovement Character { get; set; }

  public abstract bool ShouldMove(PlayerState state);
  public abstract PlayerState Move(PlayerState state, PlayerInputContext input);

}

[Serializable]
internal class GroundMovement : CharacterMover {

  public override bool ShouldMove(PlayerState state) => Character.Physics.IsGrounded;

  public override PlayerState Move(PlayerState state, PlayerInputContext input) {
    state.IsFastFalling = false;
    state.JumpCount = 0;
    Character.ApplyControlledMovement(ref state, input.Movement.Value);
    return state;
  }

}

[Serializable]
internal class AerialMovement : CharacterMover {

  public override bool ShouldMove(PlayerState state) => !Character.Physics.IsGrounded;

  public override PlayerState Move(PlayerState state, PlayerInputContext input) {
    if (input.Smash.Value.y < -DirectionalInput.DeadZone) {
      state.IsFastFalling = true;
    }
    Character.ApplyControlledMovement(ref state, input.Movement.Value);
    return state;
  }

}

[Serializable]
internal class LedgeMovement : CharacterMover {

  public override bool ShouldMove(PlayerState state) => state.IsGrabbingLedge;

  public override PlayerState Move(PlayerState state, PlayerInputContext input) {
    state.Velocity = Vector3.zero;
    if (input.Movement.Direction == Direction.Down) {
      state.ReleaseLedge();
    }
    return state;
  }

}

[Serializable]
internal class HitstunMovement : CharacterMover {

  public override bool ShouldMove(PlayerState state) => state.IsHit;

  public override PlayerState Move(PlayerState state, PlayerInputContext input) => state;

}

[Serializable]
internal class RespawnMovement : CharacterMover {

  public override bool ShouldMove(PlayerState state) => state.IsRespawning;

  public override PlayerState Move(PlayerState state, PlayerInputContext input) {
    state.Velocity = Vector3.zero;
    if (input.Movement.Direction != Direction.Neutral) {
      state.RespawnTimeRemaining = 0;
    }
    return state;
  }

}

}
