using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterStateMachine : MonoBehaviour, IPlayerSimulation, IPlayerView {

  public CharacterControllerBuilder States;
  public CharacterPhysics Physics;

  public StateController<CharacterState, CharacterContext> StateController { get; private set; }

  public CharacterStateData StateData => StateController?.CurrentState?.Data;

  Dictionary<uint, CharacterState> stateMap;
  CharacterContext context = new CharacterContext();

  public Task Initialize(PlayerConfig config, bool isView = false) {
    States = Instantiate(States); // Create a per-player copy of the builder.
    StateController = States.BuildCharacterControllerImpl(new StateControllerBuilder<CharacterState, CharacterContext>());
    stateMap = StateController.States.ToDictionary(s => s.Id, s => s);
    return Task.WhenAll(stateMap.Values.Select(s => s.Initalize(config, gameObject, isView)).Where(t => t != null));
  }

  public void Presimulate(PlayerState state) => ApplyState(state);

  public void ApplyState(PlayerState state) {
    CharacterState controllerState;
    if (stateMap.TryGetValue(state.StateID, out controllerState)) {
      StateController.SetState(controllerState);
      controllerState.ApplyState(state);
    }
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    context.State = state;
    context.Input = input;
    context.IsGrounded = Physics.IsGrounded;
    context.CanJump = state.RemainingJumps > 0;
    context.StateLength = StateController.CurrentState.Data.Length;

    StateController.UpdateState(context);
    state = StateController.CurrentState.Simulate(context.State, input);

    state.StateID = StateController.CurrentState.Id;
    return state;
  }

  public PlayerState ResetState(PlayerState state) {
    state.StateID = StateController.DefaultState.Id;
    return state;
  }

}

}
