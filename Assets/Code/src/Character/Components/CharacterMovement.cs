using HouraiTeahouse.Tasks;
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(CharacterPhysics))]
public class CharacterMovement : MonoBehaviour, ICharacterSimulation {

  public CharacterPhysics Physics;

  public float[] JumpPower;

  public int MaxJumpCount {
    get { return JumpPower.Length; }
  }

  GroundMovement Ground;
  AerialMovement Aerial;
  LedgeMovement Ledge;

  public ITask Initialize(PlayerConfig config, bool isView) {
    if (Physics == null) {
      Physics = GetComponent<CharacterPhysics>();
    }

    Ground = Ground ?? new GroundMovement();
    Aerial = Aerial ?? new AerialMovement();
    Ledge = Ledge ?? new LedgeMovement();
    return Task.Resolved;
  }

  public void Presimulate(PlayerState state) {
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    if (Physics.IsGrounded) {
      return Ground.Move(state, input, this);
    } else {
      return Aerial.Move(state, input, this);
    }
  }

}

[Serializable]
internal class GroundMovement {

  public PlayerState Move(PlayerState state,
                          PlayerInputContext input,
                          CharacterMovement movement) {
    state.RemainingJumps = movement.MaxJumpCount;
    var inputMovement = input.Current.Movement;
    state.Velocity = inputMovement;
    if (inputMovement.x > 0 && !state.Direction) {
      state.Direction = false;
    } else if (inputMovement.x < 0 && state.Direction) {
      state.Direction = true;
    }
    return state;
  }

}

[Serializable]
internal class AerialMovement {

  public PlayerState Move(PlayerState state,
                          PlayerInputContext input,
                          CharacterMovement movement) {
    var inputMovement = input.Current.Movement;
    state.Velocity = inputMovement;
    if (inputMovement.x > 0 && !state.Direction) {
      state.Direction = false;
    } else if (inputMovement.x < 0 && state.Direction) {
      state.Direction = true;
    }
    return state;
  }

}

[Serializable]
internal class LedgeMovement {

  public PlayerState Move(PlayerState state,
                          PlayerInputContext input,
                          CharacterMovement movement) {
    var inputMovement = input.Current.Movement;
    state.Velocity = inputMovement;
    if (inputMovement.x > 0 && !state.Direction) {
      state.Direction = false;
    } else if (inputMovement.x < 0 && state.Direction) {
      state.Direction = true;
    }
    return state;
  }

}

}
