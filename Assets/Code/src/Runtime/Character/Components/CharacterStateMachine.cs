using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterStateMachine : MonoBehaviour, IPlayerSimulation, IPlayerView {

  public CharacterControllerBuilder States;

  public StateController<CharacterState, CharacterContext> StateController { get; private set; }

  Dictionary<uint, CharacterState> stateMap;
  CharacterContext context = new CharacterContext();

  CharacterPhysics Physics;
  CharacterMovement Movement;
  CharacterShield Shield;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Physics = GetComponentInChildren<CharacterPhysics>();
    Movement = GetComponentInChildren<CharacterMovement>();
    Shield = GetComponentInChildren<CharacterShield>();
  }

  public Task Initialize(PlayerConfig config, bool isView = false) {
    States = Instantiate(States); // Create a per-player copy of the builder.
    StateController = States.BuildCharacterControllerImpl(new StateControllerBuilder<CharacterState, CharacterContext>());
    stateMap = StateController.States.ToDictionary(s => s.Id, s => s);
    return Task.WhenAll(stateMap.Values.Select(s => s.Initalize(config, gameObject, isView)).Where(t => t != null));
  }

  public void Presimulate(PlayerState state) => ApplyState(state);

  public void ApplyState(PlayerState state) => GetControllerState(state)?.ApplyState(state);

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    var controllerState = GetControllerState(state);
    context.State = state;
    context.Input = input;
    context.ShieldBroken = Shield.IsShieldBroken(state);
    context.IsGrounded = Physics.IsGrounded;
    context.CanJump = Movement.CanJump(state);
    context.StateLength = controllerState.Data.Length;

    controllerState = StateController.UpdateState(controllerState, context);
    state = controllerState.Simulate(context.State, input);

    state.StateID = controllerState.Id;
    return state;
  }

  public CharacterState GetControllerState(PlayerState state) =>  GetControllerState(state.StateID);

  public CharacterState GetControllerState(uint id) {
    CharacterState controllerState;
    if (stateMap.TryGetValue(id, out controllerState)) {
      return controllerState;
    } else {
      return null;
    }
  }

  public PlayerState ResetState(PlayerState state) {
    state.StateID = StateController.DefaultState.Id;
    return state;
  }

}

}
