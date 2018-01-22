using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(CharacterPhysics))]
public class CharacterMovement : MonoBehaviour, IPlayerSimulation {

  public CharacterPhysics Physics;
  public CharacterStateMachine StateMachine;

  public float[] JumpPower;

  public int MaxJumpCount =>  JumpPower?.Length ?? 0;

  CharacterMover[] Movers;
  CharacterMover Ground, Aerial, Ledge, Respawn;

  public Task Initialize(PlayerConfig config, bool isView) {
    if (Physics == null) {
      Physics = GetComponent<CharacterPhysics>();
    }

    Movers = new CharacterMover[] {
      new HitstunMovement(),
      new RespawnMovement(),
      new LedgeMovement(),
      new GroundMovement(),
      new AerialMovement(),
    };

    foreach (var mover in Movers)
      mover.Character = this;

    return Task.CompletedTask;
  }
  
  public PlayerState ResetState(PlayerState state) => state;

  public void Presimulate(PlayerState state) {
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    foreach (var mover in Movers) {
      if (mover.ShouldMove(state)) {
        return mover.Move(state, input);
      }
    }
    return state;
  }

  public bool CanJump(PlayerState state) => state.RemainingJumps > 0;

  public float GetJumpPower(PlayerState state) {
    return JumpPower[JumpPower.Length - state.RemainingJumps];
  }

  public void Jump(ref PlayerState state) {
    if (CanJump(state)) {
      state.Velocity.y = GetJumpPower(state);
      state.RemainingJumps--;
    }
  }

  public void SetDirection(ref PlayerState state, bool direction) {
    if (StateMachine.StateData.CanTurn) {
      state.Direction = direction;
    }
  }

  public void ApplyControlledMovement(ref PlayerState state, Vector2 movementInput) {
    var data = StateMachine.StateData;
    switch (data.MovementType) {
      case MovementType.Normal:
        var dir = state.Direction ? 1f : -1f;
        state.Velocity.x =  dir * Mathf.Abs(movementInput.x) * data.MaxMoveSpeed;
        break;
      case MovementType.DirectionalInfluenceOnly:
        state.Velocity.x = movementInput.x * data.MaxMoveSpeed;
        break;
    }
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
    var inputMovement = input.Current.Movement;
    state.Velocity.x = inputMovement.x;
    state.IsFastFalling = false;
    state.RemainingJumps = (uint)Character.MaxJumpCount;
    var horizontalMovement = input.Movement.Value.x;
    if (horizontalMovement > DirectionalInput.DeadZone) {
      state.Direction = true;
    } else if (horizontalMovement < -DirectionalInput.DeadZone) {
      state.Direction = false;
    }
    Character.ApplyControlledMovement(ref state, input.Movement.Value);
    if (input.Jump.WasPressed) {
      Character.Jump(ref state);
    }
    return state;
  }

}

[Serializable]
internal class AerialMovement : GroundMovement {

  public override bool ShouldMove(PlayerState state) => !Character.Physics.IsGrounded;

}

[Serializable]
internal class LedgeMovement : CharacterMover {

  public override bool ShouldMove(PlayerState state) => state.IsGrabbingLedge;

  public override PlayerState Move(PlayerState state, PlayerInputContext input) {
    state.Velocity = Vector3.zero;
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
    return state;
  }

}

}
