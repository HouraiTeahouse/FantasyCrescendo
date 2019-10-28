using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class TapPlayerInputSource : IInputSource<PlayerInput> {

  class TapInputDetector {

    static InputConfig InputConfig;

    public Vector2 LastRawInput;
    public Vector2 CurrentRawInput;

    public Vector2 SmashValue;
    public int SmashFramesRemaining;

    public TapInputDetector() {
      if (InputConfig == null) {
        InputConfig = Config.Get<InputConfig>();
      }
    }

    public void Update(Vector2 newInput) {
      LastRawInput = CurrentRawInput;
      CurrentRawInput = newInput;

      SmashFramesRemaining = Mathf.Max(0, SmashFramesRemaining - 1);

      var lastInput = InputUtil.EnforceDeadZone(LastRawInput);
      var currentInput = InputUtil.EnforceDeadZone(CurrentRawInput);
      if (InputUtil.OutsideDeadZone(lastInput)) {
        SmashValue = Vector2.zero;
        return;
      } 

      var diff = currentInput - lastInput;
      diff = InputUtil.EnforceDeadZone(diff, InputConfig.SmashThreshold);
      diff = InputUtil.MaxComponent(diff);

      if (SmashFramesRemaining > 0) {
        // Has recently smashed, needs to be in a different direction to change
        var currentDirection = DirectionalInput.GetDirection(SmashValue);
        var newDirection = DirectionalInput.GetDirection(diff);
        if (currentDirection != newDirection) {
          RefreshSmashValue(diff);
        }
      } else if (!InputUtil.OutsideDeadZone(diff, InputConfig.SmashThreshold)) {
        SmashValue = Vector2.zero;
      } else {
        RefreshSmashValue(diff);
      }
    }

    void RefreshSmashValue(Vector2 value) {
      SmashValue = value;
      SmashFramesRemaining = (int)InputConfig.SmashFrameWindow;
    }

  }

  readonly TapInputDetector TapDetector;
  readonly IInputSource<PlayerInput> _baseInputSource;

  public TapPlayerInputSource(IInputSource<PlayerInput> baseInput) {
    TapDetector = new TapInputDetector();
    _baseInputSource = baseInput;
  }

  public PlayerInput SampleInput() {
    var input = _baseInputSource.SampleInput();

    // TODO(james7132): Update TapDetector to operate on FixedVector16s
    TapDetector.Update((Vector2)input.Movement);
    input.Smash = (FixedVector16)TapDetector.SmashValue; //controllerMapping.Smash(device);

    return input;
  }

}

}