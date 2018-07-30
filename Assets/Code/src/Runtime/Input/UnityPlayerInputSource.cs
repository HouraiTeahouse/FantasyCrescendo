using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;
using UnityEngine.Experimental.Input;

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
      input = new PlayerInput { IsValid = _config.IsLocal };
    } else {
      UpdatePlayerInput(devices[(int)playerId]);
    }
    return input;
  }

  void UpdatePlayerInput(Gamepad device) {
    // TODO(james7132): Add PlayerControllerMapping support
    input.IsValid = true;
    input.Movement = device.leftStick.ReadRawValue();
    input.Attack = device.buttonSouth.isPressed;
    input.Special = device.buttonEast.isPressed;
    input.Shield = device.leftTrigger.isPressed || device.rightTrigger.isPressed;
    input.Jump = device.buttonNorth.isPressed || device.buttonWest.isPressed;
  }

}

}