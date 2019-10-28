using HouraiTeahouse.Networking;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object representing the complete input of one player for a given
/// tick.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 5)]
public struct PlayerInput {

  public const int kAttackBit  = 0;
  public const int kSpecialBit = 1;
  public const int kJumpBit    = 2;
  public const int kShieldBit  = 3;
  public const int kGrabBit    = 4;

  [FieldOffset(0)] public BitArray8 Buttons;
  [FieldOffset(1)] public FixedVector16 Movement;
  [FieldOffset(3)] public FixedVector16 Smash;

  public bool Attack {
    get => Buttons[kAttackBit];
    set => Buttons[kAttackBit] = value;
  }

  public bool Special {
    get => Buttons[kSpecialBit];
    set => Buttons[kSpecialBit] = value;
  }

  public bool Jump {
    get => Buttons[kJumpBit];
    set => Buttons[kJumpBit] = value;
  }

  public bool Shield {
    get => Buttons[kJumpBit];
    set => Buttons[kJumpBit] = value;
  }

  public bool Grab {
    get => Buttons[kGrabBit];
    set => Buttons[kGrabBit] = value;
  }

  public PlayerInput MergeWith(PlayerInput other) {
    return new PlayerInput {
      Movement = Movement + other.Movement,
      Smash = Smash + other.Smash,
      Buttons = Buttons | other.Buttons
    };
  }

}

/// <summary>
/// A data object for managing the state and change of a single
/// player's input over two ticks of gameplay.
/// </summary>
public readonly struct PlayerInputContext {

  readonly BitArray8 _prev;
  readonly BitArray8 _cur;
  public readonly DirectionalInput Movement;
  public readonly DirectionalInput Smash;

  public PlayerInputContext(ref PlayerInput previous,
                            ref PlayerInput current) {
    Movement = (Vector2)current.Movement;
    Smash = (Vector2)current.Smash;
    _prev = previous.Buttons;
    _cur = current.Buttons;
  }

  public ButtonContext Attack => new ButtonContext(_prev, _cur, PlayerInput.kAttackBit);
  public ButtonContext Special => new ButtonContext(_prev, _cur, PlayerInput.kSpecialBit);
  public ButtonContext Jump => new ButtonContext(_prev, _cur, PlayerInput.kJumpBit);
  public ButtonContext Shield => new ButtonContext(_prev, _cur, PlayerInput.kShieldBit);
  public ButtonContext Grab => new ButtonContext(_prev, _cur, PlayerInput.kGrabBit);

}

public readonly struct ButtonContext {

  public readonly bool Previous;
  public readonly bool Current;

  public ButtonContext(BitArray8 previous, BitArray8 current, int bit) {
    Previous = previous[bit];
    Current = current[bit];
  }

  public bool WasPressed => !Previous && Current;
  public bool WasReleased => Previous && !Current;

}

public static class InputUtil {

  static InputConfig InputConfig;

  static InputConfig GetConfig() {
    if (InputConfig == null) {
      InputConfig = Config.Get<InputConfig>();
    }
    return InputConfig;
  }

  public static Vector2 MaxComponent(in Vector2 value) {
    if (Math.Abs(value.x) >= Mathf.Abs(value.y)) {
      return new Vector2(value.x, 0f);
    } else {
      return new Vector2(0f, value.y);
    }
  }

  public static bool OutsideDeadZone(float value, float? deadZone = null) {
    float zone = deadZone ?? GetConfig().DeadZone;
    return Mathf.Abs(value) >= zone;
  }

  public static bool OutsideDeadZone(in Vector2 value, float? deadZone = null) {
    return OutsideDeadZone(value.x, deadZone) || OutsideDeadZone(value.y, deadZone);
  }

  public static Vector2 EnforceDeadZone(in Vector2 input, float? deadZone = null) {
    return new Vector2 (
      OutsideDeadZone(input.x, deadZone) ? input.x : 0.0f,
      OutsideDeadZone(input.y, deadZone) ? input.y : 0.0f
    );
  }

}

public readonly struct DirectionalInput {

  public readonly Vector2 Value;
  public Direction Direction => GetDirection(Value);

  public DirectionalInput(in Vector2 dir) {
    Value = dir;
  }

  public static implicit operator DirectionalInput(in Vector2 dir) {
    return new DirectionalInput(dir);
  }

  public static implicit operator Vector2(DirectionalInput dir) {
    return dir.Value;
  }

  public static Direction GetDirection(in Vector2 direction ) {
    var dir = InputUtil.EnforceDeadZone(direction);
    var absX = Mathf.Abs(dir.x);
    var absY = Mathf.Abs(dir.y);
    if (absX > absY) {
      if (dir.x < 0) return Direction.Left;
      if (dir.x > 0) return Direction.Right;
    }
    if (absX <= absY) {
      if (dir.y < 0) return Direction.Down;
      if (dir.y > 0) return Direction.Up;
    }
    return Direction.Neutral;
  }

}

public enum Direction {
  Neutral,
  Up,
  Down,
  Left,
  Right
}

}
