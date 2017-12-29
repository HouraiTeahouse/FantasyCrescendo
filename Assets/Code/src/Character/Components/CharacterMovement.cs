using HouraiTeahouse.Tasks;
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(CharacterPhysics))]
public class CharacterMovement : MonoBehaviour, ICharacterSimulation {

  public CharacterPhysics Physics;

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

  public PlayerState Simulate(PlayerState state, PlayerInput input) {
    state.Velocity = input.Movement;
    if (input.Movement.x > 0 && !state.Direction) {
      state.Direction = false;
    } else if (input.Movement.x < 0 && state.Direction) {
      state.Direction = true;
    }
    return state;
    //if (Physics.IsGrounded) {
      //return Ground.Move(state, input);
    //} else {
      //return Aerial.Move(state, input);
    //}
  }

}

[Serializable]
internal class GroundMovement {

  public PlayerState Move(PlayerState state, PlayerInput input) {
    return state;
  }

}

[Serializable]
internal class AerialMovement {

  public PlayerState Move(PlayerState state, PlayerInput input) {
    return state;
  }

}

[Serializable]
internal class LedgeMovement {

  public PlayerState Move(PlayerState state, PlayerInput input) {
    return state;
  }

}

}
