using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object representing the complete input of one player for a given
/// tick.
/// </summary>
public struct PlayerInput : IValidatable {

  // One Player Total: 17 bytes
  // Four Player Total: 68 bytes
  //
  // 60 times one: 1020 bytes
  // 60 times four: 4080 bytes

  public bool IsValid;                          // 1 bit

  public Vector2 Movement;                      // 8 bytes
  public Vector2 Smash;                         // 8 bytes

  public bool Attack;                           // 1 bit
  public bool Special;                          // 1 bit
  public bool Jump;                             // 1 bit
  public bool Shield;                           // 1 bit
  public bool Grab;                             // 1 bit

  bool IValidatable.IsValid => IsValid;

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

/// <summary>
/// A simple data object for managing the state and change of a single
/// button over two ticks of gameplay.
/// </summary>
public struct ButtonContext {

  public bool Previous;
  public bool Current;

  public bool WasPressed {
    get { return !Previous && Current; }
  }

  public bool WasReleased {
    get { return Previous && !Current; }
  }

}



}
