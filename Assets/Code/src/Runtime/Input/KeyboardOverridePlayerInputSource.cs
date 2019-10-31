using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
      Debug.Log(KeyboardInput());
      input = input.MergeWith(KeyboardInput());
    }
    return input;
  }

  static PlayerInput KeyboardInput() {
    var keyboard = Keyboard.current;
    var wasd = new Vector2(ButtonAxis(Key.A, Key.D), ButtonAxis(Key.S, Key.W));
    var arrowKeys = new Vector2(ButtonAxis(Key.LeftArrow, Key.RightArrow), 
                                ButtonAxis(Key.DownArrow, Key.UpArrow));
    return new PlayerInput {
      Movement = (FixedVector16)(wasd + arrowKeys),
      Smash = (FixedVector16)wasd,
      Attack = keyboard.eKey.isPressed,
      Special = keyboard.rKey.isPressed,
      Shield = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed,
      //TODO(james7132): Make Tap Jump Configurable
      Jump = keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed
    };
  }

  static float ButtonAxis(Key neg, Key pos) {
    var keyboard = Keyboard.current;
    var val = keyboard[neg].isPressed ? -1.0f : 0.0f;
    return val + (keyboard[pos].isPressed ? 1.0f : 0.0f);
  }

}

}