using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterStateMachine : MonoBehaviour, IPlayerSimulation, IPlayerView {

  public CharacterControllerBuilder States;

  public StateController StateController { get; private set; }

  Dictionary<uint, State> stateMap;
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
    StateController = States.BuildCharacterControllerImpl(new StateControllerBuilder());
    stateMap = StateController.States.ToDictionary(s => s.Id, s => s);
    return Task.WhenAll(stateMap.Values.Select(s => s.Initalize(config, gameObject, isView)).Where(t => t != null));
  }

  public void Presimulate(ref PlayerState state) => ApplyState(ref state);

  public void ApplyState(ref PlayerState state) => GetControllerState(ref state)?.ApplyState(ref state);

  public void Simulate(ref PlayerState state, PlayerInputContext input) {
    var controllerState = GetControllerState(ref state);
    context.State = state;
    context.Input = input;
    context.ShieldBroken = Shield.IsShieldBroken(ref state);
    context.IsGrounded = Physics.IsGrounded;
    context.CanJump = Movement.CanJump(state);
    context.StateLength = controllerState.Data.Length;

    controllerState = StateController.UpdateState(controllerState, context);
    controllerState.Simulate(ref context.State, input);
    state = context.State;

    state.StateID = controllerState.Id;
  }

  public State GetControllerState(ref PlayerState state) =>  GetControllerState(state.StateID);

  public State GetControllerState(uint id) {
    State controllerState;
    if (stateMap.TryGetValue(id, out controllerState)) {
      return controllerState;
    } else {
      return null;
    }
  }

  public void ResetState(ref PlayerState state) {
    state.StateID = StateController.DefaultState.Id;
  }

}

}
