using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class InControlInputSource : IInputSource<GameInput> {

  GameConfig config;
  GameInput input;
  // TODO(james7132): Support multiple with player level configurations.
  PlayerControllerMapping controllerMapping;

  public InControlInputSource(GameConfig config) {
    this.config = config;
    controllerMapping = new PlayerControllerMapping();
    input = new GameInput(config);
    for (int i = 0; i < input.PlayerInputs.Length; i++) {
      input.PlayerInputs[i].IsValid = true;
    }
  }
  
  public GameInput SampleInput() {
    var allDevices = InputManager.Devices;
    var newInput = input.Clone();
    for (var i = 0; i < config.PlayerConfigs.Length; i++) {
      var playerConfig = config.PlayerConfigs[i];
      var playerId = playerConfig.PlayerID;
      if (playerId >= allDevices.Count) {
        newInput.PlayerInputs[i] = new PlayerInput { IsValid = true };
        continue;
      }
      UpdatePlayerInput(ref newInput.PlayerInputs[i], allDevices[(int)playerId]);
      if (playerId == 0) {
        newInput.PlayerInputs[i].Merge(KeyboardInput());
      }
    }
    input = newInput;
    return newInput;
  }

  void UpdatePlayerInput(ref PlayerInput input, InputDevice device) {
    input.Movement = controllerMapping.Movement(device);
    input.Smash = controllerMapping.Smash(device);
    input.Attack = controllerMapping.Attack(device);
    input.Special = controllerMapping.Special(device);
    input.Shield = controllerMapping.Shield(device);
    input.Jump = controllerMapping.Jump(device);
  }

  PlayerInput KeyboardInput() {
    return new PlayerInput {
      Movement = new Vector2(ButtonAxis(KeyCode.A, KeyCode.D), ButtonAxis(KeyCode.S, KeyCode.W)),
      //TODO(james7132): Make Tap Jump Configurable
      Jump = Input.GetKey(KeyCode.W),
      IsValid = true
    };
  }

  float ButtonAxis(KeyCode neg, KeyCode pos) {
    var val = Input.GetKey(neg) ? -1.0f : 0.0f;
    return val + (Input.GetKey(pos) ? 1.0f : 0.0f);
  }

}

public class PlayerControllerMapping {

  public InputControlType[] AttackTargets = new[] { InputControlType.Action1 };
  public InputControlType[] SpecialTargets = new[] { InputControlType.Action2 };
  public InputControlType[] JumpTargets = new[] { InputControlType.Action3, InputControlType.Action4 };
  public InputControlType[] ShieldTargets = new[] { InputControlType.RightTrigger, InputControlType.LeftTrigger };
  public InputControlType[] GrabTargets = new[] { InputControlType.RightBumper, InputControlType.LeftBumper };

  // TODO(james7132): Make this configurable
  public Vector2 Movement(InputDevice device) => device.LeftStick;
  // TODO(james7132): Make this configurable
  public Vector2 Smash(InputDevice device) => device.RightStick;

  public bool Attack(InputDevice device) => AnyButtonPressed(AttackTargets, device);
  public bool Special(InputDevice device) => AnyButtonPressed(SpecialTargets, device);
  public bool Jump(InputDevice device) => AnyButtonPressed(JumpTargets, device);
  public bool Shield(InputDevice device) => AnyButtonPressed(ShieldTargets, device);
  public bool Grab(InputDevice device) => AnyButtonPressed(GrabTargets, device);

  bool AnyButtonPressed(InputControlType[] targetSet, InputDevice device) {
    for (int i = 0; i < targetSet.Length; i++) {
      if (device.GetControl(targetSet[i])) {
        return true;
      }
    }
    return false;
  }

}

}
