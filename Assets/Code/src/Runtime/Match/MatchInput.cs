using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public struct MatchInput {

  public PlayerInput[] PlayerInputs;

  public MatchInput(MatchConfig config) {
    PlayerInputs = new PlayerInput[config.PlayerCount];
  }

  public bool IsValid => PlayerInputs.IsAllValid();

  public MatchInput Predict(MatchInput baseInput) {
    var clone = Clone();
    ForceValid(clone.PlayerInputs, true);
    return clone;
  }

  public void MergeWith(MatchInput otherInput) {
    Assert.AreEqual(PlayerInputs.Length, otherInput.PlayerInputs.Length);
    for (int i = 0; i < PlayerInputs.Length; i++) {
      if (!PlayerInputs[i].IsValid && otherInput.PlayerInputs[i].IsValid) {
        PlayerInputs[i] = otherInput.PlayerInputs[i];
      }
    }
  }

  internal void Reset() {
    ForceValid(PlayerInputs, false);
  }

  static void ForceValid(PlayerInput[] inputs, bool valid) {
    for (int i = 0; i < inputs.Length; i++) {
      inputs[i].IsValid = valid;
    }
  }

  public MatchInput Clone() {
    MatchInput clone = this;
    clone.PlayerInputs = (PlayerInput[]) PlayerInputs.Clone();
    return clone;
  }

}

public class MatchInputContext {

  public PlayerInputContext[] PlayerInputs;

  public MatchInputContext(MatchInput input) {
    Reset(input);
  }

  public bool IsValid {
    get {
      bool isValid = true;
      for (int i = 0; i < PlayerInputs.Length; i++) {
        isValid &= PlayerInputs[i].IsValid;
      }
      return isValid;
    }
  }

  public void Reset(MatchInput current) {
    PlayerInputs = new PlayerInputContext[current.PlayerInputs.Length];
    for (int i = 0; i < PlayerInputs.Length; i++) {
      PlayerInputs[i] = new PlayerInputContext {
        Current = current.PlayerInputs[i]
      };
    }
  }

  public void Reset(MatchInput previous, MatchInput current) {
    Reset(previous);
    Update(current);
  }

  public void Update(MatchInput input) {
    for (int i = 0; i < input.PlayerInputs.Length; i++) {
      PlayerInputs[i].Update(input.PlayerInputs[i]);
    }
  }

}

}
