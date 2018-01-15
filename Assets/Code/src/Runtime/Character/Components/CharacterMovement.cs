using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(CharacterPhysics))]
public class CharacterMovement : MonoBehaviour, IPlayerSimulation {

  public CharacterPhysics Physics;

  public float[] JumpPower;

  public int MaxJumpCount =>  JumpPower?.Length ?? 0;

  GroundMovement Ground;
  AerialMovement Aerial;
  LedgeMovement Ledge;
  RespawnMovement Respawn;

  public Task Initialize(PlayerConfig config, bool isView) {
    if (Physics == null) {
      Physics = GetComponent<CharacterPhysics>();
    }

    Ground = Ground ?? new GroundMovement();
    Aerial = Aerial ?? new AerialMovement();
    Ledge = Ledge ?? new LedgeMovement();
    Respawn = Respawn ?? new RespawnMovement();

    return Task.CompletedTask;
  }
  
  public PlayerState ResetState(PlayerState state) => state;

  public void Presimulate(PlayerState state) {
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    ICharacterMovement mover = Aerial;
    if (state.IsRespawning) {
      mover = Respawn;
    } else if (state.IsGrabbingLedge) {
      mover = Ledge;
    } else if (Physics.IsGrounded) {
      mover = Ground;
    }
    return mover.Move(state, input, this);
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

}

public interface ICharacterMovement {
  PlayerState Move(PlayerState state,
                   PlayerInputContext input,
                   CharacterMovement movement);
}

[Serializable]
internal class GroundMovement : ICharacterMovement {

  public PlayerState Move(PlayerState state,
                          PlayerInputContext input,
                          CharacterMovement movement) {
    state.RemainingJumps = movement.MaxJumpCount;
    var inputMovement = input.Current.Movement;
    state.Velocity.x = inputMovement.x;
    if (inputMovement.x > 0 && !state.Direction) {
      state.Direction = false;
    } else if (inputMovement.x < 0 && state.Direction) {
      state.Direction = true;
    }
    if (input.Jump.WasPressed) {
      movement.Jump(ref state);
    }
    return state;
  }

}

[Serializable]
internal class AerialMovement : ICharacterMovement {

  public PlayerState Move(PlayerState state,
                          PlayerInputContext input,
                          CharacterMovement movement) {
    var inputMovement = input.Current.Movement;
    state.Velocity.x = inputMovement.x;
    if (input.Jump.WasPressed) {
      movement.Jump(ref state);
    }
    return state;
  }

}

[Serializable]
internal class LedgeMovement : ICharacterMovement {

  public PlayerState Move(PlayerState state,
                          PlayerInputContext input,
                          CharacterMovement movement) {
    return state;
  }

}

[Serializable]
internal class RespawnMovement : ICharacterMovement {

  public PlayerState Move(PlayerState state,
                          PlayerInputContext input,
                          CharacterMovement movement) {
    state.Velocity = Vector3.zero;
    return state;
  }

}

}
