using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = HouraiTeahouse.FantasyCrescendo.Players.PlayerInput;

namespace HouraiTeahouse.FantasyCrescendo {

public class UnityPlayerInputSource : IInputSource<PlayerInput> {

  readonly PlayerConfig _config;
  PlayerInput input;

  public UnityPlayerInputSource(PlayerConfig config) {
    _config = config;
  }

  public PlayerInput SampleInput() {
    var devices = Gamepad.all;
    var playerId = _config.LocalPlayerID;
    if (!_config.IsLocal || playerId >= devices.Count) {
      input = new PlayerInput();
    } else {
      UpdatePlayerInput(devices[(int)playerId]);
    }
    return input;
  }

  void UpdatePlayerInput(Gamepad device) {
    // TODO(james7132): Add PlayerControllerMapping support
    input.Movement = device.leftStick.ReadValue();
    input.Attack = device.buttonSouth.isPressed;
    input.Special = device.buttonEast.isPressed;
    input.Shield = device.leftTrigger.isPressed || device.rightTrigger.isPressed;
    input.Jump = device.buttonNorth.isPressed || device.buttonWest.isPressed;
  }

}

}