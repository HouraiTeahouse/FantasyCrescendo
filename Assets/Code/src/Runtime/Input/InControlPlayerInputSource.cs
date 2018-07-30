using HouraiTeahouse.FantasyCrescendo.Players;
using InControl;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class InControlPlayerInputSource : IInputSource<PlayerInput> {

  readonly PlayerControllerMapping controllerMapping;
  readonly PlayerConfig _config;
  PlayerInput input;

  public InControlPlayerInputSource(PlayerConfig config) {
    _config = config;
    // TODO(james7132): Make this configurable
    controllerMapping = new PlayerControllerMapping();
  }

  public PlayerInput SampleInput() {
    var devices = InputManager.Devices;
    var playerId = _config.LocalPlayerID;
    if (!_config.IsLocal || playerId >= devices.Count) {
      input = new PlayerInput { IsValid = _config.IsLocal };
    } else {
      UpdatePlayerInput(devices[(int)playerId]);
    }
    return input;
  }

  void UpdatePlayerInput(InputDevice device) {
    input.IsValid = true;
    input.Movement = controllerMapping.Movement(device);
    input.Attack = controllerMapping.Attack(device);
    input.Special = controllerMapping.Special(device);
    input.Shield = controllerMapping.Shield(device);
    input.Jump = controllerMapping.Jump(device);
  }

}

}