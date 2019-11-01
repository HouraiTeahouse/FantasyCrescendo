using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class UnityPlayerInputSource : IInputSource<PlayerInput> {

  readonly PlayerConfig _config;

  public UnityPlayerInputSource(PlayerConfig config) {
    _config = config;
  }

  public PlayerInput SampleInput() {
    var manager = InputManager.Instance;
    Assert.IsNotNull(manager);
    return manager.GetInputForPlayer(_config.LocalPlayerID);
  }

}

}
