using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public struct GameInput {

  public PlayerInput[] PlayerInputs;

  public GameInput(GameConfig config) {
    PlayerInputs = new PlayerInput[config.PlayerConfigs.Length];
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

  public GameInput Predict(GameInput baseInput) {
    var clone = Clone();
    ForceValid(clone.PlayerInputs, true);
    return clone;
  }

  public void MergeWith(GameInput otherInput) {
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

  public GameInput Clone() {
    GameInput clone = this;
    clone.PlayerInputs = (PlayerInput[]) PlayerInputs.Clone();
    return clone;
  }

}

}
