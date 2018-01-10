using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterContext {
  public PlayerState State;
  public PlayerInputContext Input;

  // Local computed state information
  public float Direction => State.Direction ? 1.0f : -1.0f;
  public bool IsGrounded;
  public bool CanJump;

  public CharacterContext Clone() {
    return (CharacterContext) MemberwiseClone();
  }
}

}
