using HouraiTeahouse.Networking;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object representing the complete input of one player for a given
/// tick.
/// </summary>
public struct PlayerInput {

  public const int kAttackBit  = 0;
  public const int kSpecialBit = 1;
  public const int kJumpBit    = 2;
  public const int kShieldBit  = 3;
  public const int kGrabBit    = 4;

  public byte Buttons;
  public Vector2b Movement;
  public Vector2b Smash;

  public bool Attack {
    get => BitUtil.GetBit(Buttons, kAttackBit);
    set => BitUtil.SetBit(ref Buttons, kAttackBit, value);
  }

  public bool Special {
    get => BitUtil.GetBit(Buttons, kSpecialBit);
    set => BitUtil.SetBit(ref Buttons, kSpecialBit, value);
  }

  public bool Jump {
    get => BitUtil.GetBit(Buttons, kJumpBit);
    set => BitUtil.SetBit(ref Buttons, kJumpBit, value);
  }

  public bool Shield {
    get => BitUtil.GetBit(Buttons, kJumpBit);
    set => BitUtil.SetBit(ref Buttons, kJumpBit, value);
  }

  public bool Grab {
    get => BitUtil.GetBit(Buttons, kGrabBit);
    set => BitUtil.SetBit(ref Buttons, kGrabBit, value);
  }

  public PlayerInput MergeWith(PlayerInput other) {
    return new PlayerInput {
      Movement = (Vector2)Movement + (Vector2)other.Movement,
      Smash = (Vector2)Smash + (Vector2)other.Smash,
      Buttons = (byte)((Buttons | other.Buttons) & 31)
    };
  }

}

/// <summary>
/// A data object for managing the state and change of a single
/// player's input over two ticks of gameplay.
/// </summary>
public readonly struct PlayerInputContext {

  readonly byte _prev;
  readonly byte _cur;
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

  public ButtonContext(byte previous, byte current, int bit) {
    Previous = BitUtil.GetBit(previous, bit);
    Current = BitUtil.GetBit(current, bit);
  }

  public bool WasPressed => !Previous && Current;
  public bool WasReleased => Previous && !Current;

}

public struct Vector2b {
  public byte X;
  public byte Y;

  public float x  {
    get => ToFloat(X);
    set => X = FromFloat(value);
  }
  public float y  {
    get => ToFloat(Y);
    set => Y = FromFloat(value);
  }

  public static implicit operator Vector2b(Vector2 vector) => new Vector2b { x = vector.x, y = vector.y };
  public static implicit operator Vector2(Vector2b vector) => new Vector2 { x = vector.x, y = vector.y };

  float ToFloat(byte val) => (val - 127) / 127f;
  byte FromFloat(float val) => (byte)(Mathf.Clamp(val, -1, 1) * 127f + 127);

  public static bool operator ==(Vector2b a, Vector2b b) => a.X == b.X && a.Y == b.Y;
  public static bool operator !=(Vector2b a, Vector2b b) => !(a == b);

  public override int GetHashCode() => unchecked(31 * X + Y);
  
  public override string ToString() => $"Vector2b<{X - 128}, {Y - 128}>";

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
