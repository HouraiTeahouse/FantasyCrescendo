using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using InControl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class InControlInputSource : IInputSource {

  public byte ValidMask { get; }

  MatchInput input;
  // TODO(james7132): Support multiple with player level configurations.
  readonly PlayerControllerMapping controllerMapping;
  readonly MatchConfig config;

  public InControlInputSource(MatchConfig config) {
    this.config = config;
    controllerMapping = new PlayerControllerMapping();
    input = new MatchInput(config);
    ValidMask = 0;
    for (var i = 0; i < config.PlayerCount; i++) {
      if (!config.PlayerConfigs[i].IsLocal) continue;
      ValidMask |= (byte)(1 << i);
    }
    Debug.Log($"Valid Mask: {ValidMask}");
  }
  
  public MatchInput SampleInput() {
    var devices = InputManager.Devices;
    input = new MatchInput(input.PlayerCount);
    for (var i = 0; i < input.PlayerCount; i++) {
      var playerConfig = config.PlayerConfigs[i];
      var playerId = playerConfig.LocalPlayerID;
      if (!playerConfig.IsLocal || playerId >= devices.Count) {
        input[i] = new PlayerInput { IsValid = playerConfig.IsLocal };
        continue;
      }
      input[i] = UpdatePlayerInput(input[i], devices[(int)playerId]);
      if (playerId == 0) {
        input[i] = input[i].MergeWith(KeyboardInput());
      }
    }
    return input;
  }

  PlayerInput UpdatePlayerInput(PlayerInput input, InputDevice device) {
    input.IsValid = true;
    input.Movement = controllerMapping.Movement(device);
    input.Smash = controllerMapping.Smash(device);
    input.Attack = controllerMapping.Attack(device);
    input.Special = controllerMapping.Special(device);
    input.Shield = controllerMapping.Shield(device);
    input.Jump = controllerMapping.Jump(device);
    return input;
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
