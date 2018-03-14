using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class TestInputSource : IMatchInputSource {

  public byte ValidMask => MatchInput.AllValid;

  MatchInput input;

  public TestInputSource(MatchConfig config) {
    input = new MatchInput(config);
    for (int i = 0; i < input.PlayerCount; i++) {
      var playerInput = input[i];
      playerInput.IsValid = true;
      input[i] = playerInput;
    }
  }

  public MatchInput SampleInput() {
    var playerInput = new PlayerInput {
      Movement = new Vector2(ButtonAxis(KeyCode.A, KeyCode.D), ButtonAxis(KeyCode.S, KeyCode.W)),
      //TODO(james7132): Make Tap Jump Configurable
      Jump = Input.GetKey(KeyCode.W),
      IsValid = true
    };
    var inputValue = input;
    for (int i = 0; i < inputValue.PlayerCount; i++) {
      if (i == 0) {
        inputValue[i] = playerInput;
      }
    }
    return inputValue;
  }

  float ButtonAxis(KeyCode neg, KeyCode pos) {
    var val = Input.GetKey(neg) ? -1.0f : 0.0f;
    return val + (Input.GetKey(pos) ? 1.0f : 0.0f);
  }

}

}
