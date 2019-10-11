using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class KeyboardOverridePlayerInputSource : IInputSource<PlayerInput> {

  readonly PlayerConfig _config;
  readonly IInputSource<PlayerInput> _baseInputSource;

  public KeyboardOverridePlayerInputSource(IInputSource<PlayerInput> inputSource) {
    _baseInputSource = inputSource;
  }

  public PlayerInput SampleInput() {
    var input = _baseInputSource.SampleInput();
    if (_config.LocalPlayerID == 0) {
      input = input.MergeWith(KeyboardInput());
    }
    return input;
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
    };
  }

  static float ButtonAxis(KeyCode neg, KeyCode pos) {
    var val = Input.GetKey(neg) ? -1.0f : 0.0f;
    return val + (Input.GetKey(pos) ? 1.0f : 0.0f);
  }

}

}