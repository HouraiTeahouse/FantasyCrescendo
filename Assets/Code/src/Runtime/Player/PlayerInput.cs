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
    get { return (Buttons & 1) != 0; }
    set{ Buttons = (byte)(value ? (Buttons | 1) : (Buttons & ~1)); }
  }

  public bool Attack {
    get { return (Buttons & 2) != 0; }
    set{ Buttons = (byte)(value ? (Buttons | 2) : (Buttons & ~2)); }
  }

  public bool Special {
    get { return (Buttons & 4) != 0; }
    set{ Buttons = (byte)(value ? (Buttons | 4) : (Buttons & ~4)); }
  }

  public bool Jump {
    get { return (Buttons & 8) != 0; }
    set{ Buttons = (byte)(value ? (Buttons | 8) : (Buttons & ~8)); }
  }

  public bool Shield {
    get { return (Buttons & 16) != 0; }
    set{ Buttons = (byte)(value ? (Buttons | 16) : (Buttons & ~16)); }
  }

  public bool Grab {
    get { return (Buttons & 32) != 0; }
    set{ Buttons = (byte)(value ? (Buttons | 32) : (Buttons & ~32)); }
  }

  bool IValidatable.IsValid => IsValid;

  public void MergeWith(PlayerInput other) {
    IsValid = IsValid || other.IsValid;
    Movement = (Vector2)Movement + (Vector2)other.Movement;
    Smash = (Vector2)Smash + (Vector2)other.Smash;
    Buttons |= other.Buttons;
  }

  public override bool Equals(object obj) {
    if (!(obj is PlayerInput)) return false;
    var other = (PlayerInput)obj;
    if (!IsValid && !other.IsValid) return true;
    var equals = Movement.Equals(other.Movement) && Smash.Equals(other.Smash);
    return equals && Buttons == other.Buttons;
  }

  public override int GetHashCode() => 31 * Movement.GetHashCode() + 17 * Smash.GetHashCode() + Buttons.GetHashCode();

  public void Serialize(NetworkWriter writer) {
    var cutMovement = Movement.X == 0 && Movement.Y == 0;
    var cutSmash = Smash.X == 0 && Smash.Y == 0;
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
    writer.Write(Buttons);
    if (!cutMovement) {
      writer.Write(Movement.X);
      writer.Write(Movement.Y);
    }
    if (!cutSmash) {
      writer.Write(Smash.X);
      writer.Write(Smash.Y);
    }
  }

  public static PlayerInput Deserialize(NetworkReader reader) {
    var input = new PlayerInput();
    input.Buttons = reader.ReadByte();
    if ((input.Buttons & 64) != 0) {
      input.Movement.X = reader.ReadSByte();
      input.Movement.Y = reader.ReadSByte();
    } else {
      input.Movement.X = input.Movement.Y = 0;
    }
    if ((input.Buttons & 128) != 0) {
      input.Smash.X = reader.ReadSByte();
      input.Smash.Y = reader.ReadSByte();
    } else {
      input.Smash.X = input.Smash.Y = 0;
    }
    return input;
  }

}

/// <summary>
/// A data object for managing the state and change of a single
/// player's input over two ticks of gameplay.
/// </summary>
public class PlayerInputContext : IValidatable {

  public PlayerInput Previous;
  public PlayerInput Current;

  public void Update(PlayerInput input) {
    Previous = Current;
    Current = input;
  }

  public bool IsValid => Previous.IsValid && Current.IsValid;

  public DirectionalInput Movement => (Vector2)Current.Movement;
  public DirectionalInput Smash => (Vector2)Current.Smash;

  public ButtonContext Attack {
    get {
      return new ButtonContext {
        Previous = Previous.Attack,
        Current  = Current.Attack
      };
    }
  }

  public ButtonContext Special {
    get {
      return new ButtonContext {
        Previous = Previous.Special,
        Current = Current.Special
      };
    }
  }

  public ButtonContext Jump {
    get {
      return new ButtonContext {
        Previous = Previous.Jump,
        Current = Current.Jump
      };
    }
  }

  public ButtonContext Shield {
    get {
      return new ButtonContext {
        Previous = Previous.Shield,
        Current = Current.Shield
      };
    }
  }

  public ButtonContext Grab {
    get {
      return new ButtonContext {
        Previous = Previous.Grab,
        Current = Current.Grab
      };
    }
  }

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

}

/// <summary>
/// A simple data object for managing the state and change of a single
/// button over two ticks of gameplay.
/// </summary>
public struct ButtonContext {

  public bool Previous;
  public bool Current;

  public bool WasPressed => !Previous && Current;
  public bool WasReleased => Previous && !Current;

}

public struct DirectionalInput {

  // TODO(james7132): Move this to a Config
  public const float DeadZone = 0.3f;

  public Vector2 Value;
  public Direction Direction {
    get {
      var absX = Mathf.Abs(Value.x);
      var absY = Mathf.Abs(Value.y);
      if (absX > absY) {
        if (Value.x < -DeadZone) return Direction.Left;
        if (Value.x > DeadZone) return Direction.Right;
      }
      if (absX <= absY) {
        if (Value.y < -DeadZone) return Direction.Down;
        if (Value.y > DeadZone) return Direction.Up;
      }
      return Direction.Neutral;
    }
  }

  public static implicit operator DirectionalInput(Vector2 dir) {
    return new DirectionalInput {
      Value = dir
    };
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
