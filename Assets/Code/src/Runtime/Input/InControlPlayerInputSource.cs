using HouraiTeahouse.FantasyCrescendo.Players;
using InControl;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class InControlPlayerInputSource : IInputSource<PlayerInput> {

  readonly PlayerControllerMapping controllerMapping;
  readonly PlayerConfig config;
  readonly SmashInputDetector smashInputDetector;
  PlayerInput input;

  public InControlPlayerInputSource(PlayerConfig config) {
    this.config = config;
    // TODO(james7132): Make these configurable
    controllerMapping = new PlayerControllerMapping();
    smashInputDetector = new SmashInputDetector(5);
  }

  public PlayerInput SampleInput() {
    var devices = InputManager.Devices;
    var playerId = config.LocalPlayerID;
    if (!config.IsLocal || playerId >= devices.Count) {
      input = new PlayerInput { IsValid = config.IsLocal };
    } else {
      UpdatePlayerInput(devices[(int)playerId]);
    }
    if (playerId == 0) {
      input = input.MergeWith(KeyboardInput());
    }
    UpdateSmashFromMovement();
    return input;
  }

  void UpdatePlayerInput(InputDevice device) {
    input.IsValid = true;
    input.Movement = controllerMapping.Movement(device);
    input.Smash = controllerMapping.Smash(device);
    input.Attack = controllerMapping.Attack(device);
    input.Special = controllerMapping.Special(device);
    input.Shield = controllerMapping.Shield(device);
    input.Jump = controllerMapping.Jump(device);
  }

  void UpdateSmashFromMovement() {
    smashInputDetector.Update(input.Movement);
    var test  = config.LocalPlayerID == 0;
    var smash = smashInputDetector.GetSmash(input.Movement);
    if (test && smash != Vector2.zero) {
      Debug.Log($"{smash} {DirectionalInput.GetDirection(smash)}");
    }
    input.Smash = MergeVectors(input.Smash, smash);
  }

  static PlayerInput KeyboardInput() {
    var wasd = new Vector2(ButtonAxis(KeyCode.A, KeyCode.D), ButtonAxis(KeyCode.S, KeyCode.W));
    var arrowKeys = new Vector2(ButtonAxis(KeyCode.LeftArrow, KeyCode.RightArrow), 
                                ButtonAxis(KeyCode.DownArrow, KeyCode.UpArrow));
    return new PlayerInput {
      Movement = wasd + arrowKeys,
      Smash = wasd,
      Attack = Input.GetKey(KeyCode.E),
      Special = Input.GetKey(KeyCode.R),
      Shield = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
      //TODO(james7132): Make Tap Jump Configurable
      Jump = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow),
      IsValid = true
    };
  }

  static Vector2 MergeVectors(Vector2 a, Vector2 b) {
    return new Vector2 {
      x = Mathf.Abs(a.x) > Mathf.Abs(b.x) ? a.x : b.x,
      y = Mathf.Abs(a.y) > Mathf.Abs(b.y) ? a.y : b.y
    };
  }

  static float ButtonAxis(KeyCode neg, KeyCode pos) {
    var val = Input.GetKey(neg) ? -1.0f : 0.0f;
    return val + (Input.GetKey(pos) ? 1.0f : 0.0f);
  }

}

}