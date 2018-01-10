using HouraiTeahouse.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterStateMachine : MonoBehaviour, ICharacterSimulation, ICharacterView {

  public CharacterControllerBuilder States;
  public CharacterPhysics Physics;

  public StateController<CharacterState, CharacterContext> StateController { get; private set; }

  public CharacterStateData StateData => StateController?.CurrentState?.Data;

  Dictionary<int, CharacterState> stateMap;
  CharacterContext context = new CharacterContext();

  public ITask Initialize(PlayerConfig config, bool isView = false) {
    StateController = States.BuildCharacterControllerImpl(new StateControllerBuilder<CharacterState, CharacterContext>());
    stateMap = StateController.States.ToDictionary(s => s.AnimatorHash, s => s);
    return Task.Resolved;
  }

  public void Presimulate(PlayerState state) => ApplyState(state);

  public void ApplyState(PlayerState state) => StateController.SetState(stateMap[state.StateHash]);

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    context.State = state;
    context.Input = input;
    context.IsGrounded = Physics.IsGrounded;
    context.CanJump = state.RemainingJumps > 0;

    StateController.UpdateState(context);

    state.StateHash = StateController.CurrentState.AnimatorHash;
    return state;
  }

}

}
