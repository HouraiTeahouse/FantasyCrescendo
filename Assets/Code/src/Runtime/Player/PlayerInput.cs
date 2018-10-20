using HouraiTeahouse.FantasyCrescendo.Networking;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Players {

/// <summary>
/// A data object representing the complete input of one player for a given
/// tick.
/// </summary>
public struct PlayerInput : IValidatable, IMergable<PlayerInput> {

  // One Player Total: 5 bytes
  // Four Player Total: 20 bytes
  //
  // 60 times one: 300 bytes
  // 60 times four: 1200 bytes

  public Vector2b Movement;                     // 2 bytes
  public Vector2b Smash;                        // 2 bytes
  public byte Buttons;                          // 1 byte

  // TODO(james7132): Benchmark using AggressiveInlining on these
  public bool IsValid {
    get { return GetBit(Buttons, 0); }
    set { SetBit(ref Buttons, 0, value); }
  }

  public bool Attack {
    get { return GetBit(Buttons, 1); }
    set { SetBit(ref Buttons, 1, value); }
  }

  public bool Special {
    get { return GetBit(Buttons, 2); }
    set { SetBit(ref Buttons, 2, value); }
  }

  public bool Jump {
    get { return GetBit(Buttons, 3); }
    set { SetBit(ref Buttons, 3, value); }
  }

  public bool Shield {
    get { return GetBit(Buttons, 4); }
    set { SetBit(ref Buttons, 4, value); }
  }

  public bool Grab {
    get { return GetBit(Buttons, 5); }
    set { SetBit(ref Buttons, 5, value); }
  }

  static bool GetBit(byte value, int bit) {
    return (value & (1 << bit)) != 0;
  }

  static void SetBit(ref byte value, int bit, bool bitValue) {
    var mask = 1 << bit;
    value = (byte)(bitValue ? (value | mask) : (value & ~mask));
  }

  bool IValidatable.IsValid => IsValid;

  public PlayerInput MergeWith(PlayerInput other) {
    return new PlayerInput {
      IsValid = IsValid || other.IsValid,
      Movement = (Vector2)Movement + (Vector2)other.Movement,
      Smash = (Vector2)Smash + (Vector2)other.Smash,
      Buttons = (byte)(Buttons | other.Buttons)
    };
  }

  public override bool Equals(object obj) {
    if (!(obj is PlayerInput)) return false;
    var other = (PlayerInput)obj;
    if (!IsValid && !other.IsValid) return true;
    var equals = IsValid == other.IsValid;
    equals &= Movement.Equals(other.Movement);
    equals &= Smash.Equals(other.Smash);
    equals &= (Buttons & 63) == (other.Buttons & 63);
    return equals;
  }

  public override string ToString() {
    var buttons = Convert.ToString(Buttons, 2).PadLeft(8, '0');
    return $"PlayerInput(({Movement.x}, {Movement.y}), ({Smash.x}, {Smash.y}), {buttons})";
  }

  public override int GetHashCode() => 31 * Movement.GetHashCode() + 17 * Smash.GetHashCode() + Buttons.GetHashCode();

  public void Serialize(Serializer serializer, PlayerInput? previous = null) {
    bool cutMovement, cutSmash;
    if (!IsValid) {
      serializer.Write((byte)0);
      return;
    }
    if (previous != null && previous.Value.IsValid) {
      cutMovement = Movement.Equals(previous.Value.Movement);
      cutSmash = Smash.Equals(previous.Value.Smash);
    } else {
      cutMovement = Movement.X == 0 && Movement.Y == 0;
      cutSmash = Smash.X == 0 && Smash.Y == 0;
    }
    unchecked {
      if (cutMovement) {
        Buttons &= (byte)~64;
      } else  {
        Buttons |= 64;
      }
      if (cutSmash) {
        Buttons &= (byte)~128;
      } else  {
        Buttons |= 128;
      }
    }
    serializer.Write(Buttons);
    if (!cutMovement) {
      serializer.Write(Movement.X);
      serializer.Write(Movement.Y);
    }
    if (!cutSmash) {
      serializer.Write(Smash.X);
      serializer.Write(Smash.Y);
    }
  }

  public static PlayerInput Deserialize(Deserializer deserializer, PlayerInput? previous = null) {
    var input = new PlayerInput();
    input.Buttons = deserializer.ReadByte();
    if (!input.IsValid) return input;
    if ((input.Buttons & 64) != 0) {
      input.Movement.X = deserializer.ReadSByte();
      input.Movement.Y = deserializer.ReadSByte();
    } else {
      if (previous != null && previous.Value.IsValid) {
        input.Movement = previous.Value.Movement;
      } else {
        input.Movement.X = input.Movement.Y = 0;
      }
    }
    if ((input.Buttons & 128) != 0) {
      input.Smash.X = deserializer.ReadSByte();
      input.Smash.Y = deserializer.ReadSByte();
    } else {
      if (previous != null && previous.Value.IsValid) {
        input.Smash = previous.Value.Smash;
      } else {
        input.Smash.X = input.Smash.Y = 0;
      }
    }
    return input;
  }

}

/// <summary>
/// A data object for managing the state and change of a single
/// player's input over two ticks of gameplay.
/// </summary>
public struct PlayerInputContext : IValidatable {

  public PlayerInput Previous;
  public PlayerInput Current;

  public void Update(PlayerInput input) {
    Previous = Current;
    Current = input;
  }
  
  public void ForceValid(bool valid) {
    Previous.IsValid = valid;
    Current.IsValid = valid;
  }

  public bool IsValid => Previous.IsValid && Current.IsValid;

  public DirectionalInput Movement => (Vector2)Current.Movement;
  public DirectionalInput Smash => (Vector2)Current.Smash;

  public ButtonContext Attack => new ButtonContext(Previous.Attack, Current.Attack);
  public ButtonContext Special => new ButtonContext(Previous.Special, Current.Special);
  public ButtonContext Jump => new ButtonContext(Previous.Jump, Current.Jump);
  public ButtonContext Shield => new ButtonContext(Previous.Shield, Current.Shield);
  public ButtonContext Grab => new ButtonContext(Previous.Grab, Current.Grab);

}

public struct Vector2b {
  public sbyte X;
  public sbyte Y;

  public float x  {
    get { return ToFloat(X); }
    set { X = FromFloat(value); }
  }
  public float y  {
    get { return ToFloat(Y); }
    set { Y = FromFloat(value); }
  }

  public static implicit operator Vector2b(Vector2 vector) => new Vector2b { x = vector.x, y = vector.y };
  public static implicit operator Vector2(Vector2b vector) => new Vector2 { x = vector.x, y = vector.y };

  float ToFloat(sbyte val) => (float)val / sbyte.MaxValue;
  sbyte FromFloat(float val) => (sbyte)(Mathf.Clamp(val, -1, 1) * sbyte.MaxValue);

  public override bool Equals(object obj) {
    if (!(obj is Vector2b)) return false;
    var other = (Vector2b)obj;
    return X == other.X && Y == other.Y;
  }

  public override int GetHashCode() => unchecked(31 * X + Y);
  
  public override string ToString() => $"Vector2b<{(Vector2)this}>";

}

/// <summary>
/// A simple data object for managing the state and change of a single
/// button over two ticks of gameplay.
/// </summary>
public readonly struct ButtonContext {

  public readonly bool Previous;
  public readonly bool Current;

  public ButtonContext(bool previous, bool current) {
    Previous = previous;
    Current = current;
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
