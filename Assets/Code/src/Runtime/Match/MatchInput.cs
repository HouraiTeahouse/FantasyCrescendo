using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public struct MatchInput : IMergable<MatchInput> {

  public PlayerInput[] PlayerInputs;

  public int PlayerCount => PlayerInputs?.Length ?? 0;

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
    Assert.IsTrue(PlayerInputs.Length >= otherInput.PlayerInputs.Length);
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

  public override bool Equals(object obj) {
    if (typeof(MatchInput) != obj.GetType()) return false;
    return ArrayUtil.AreEqual(PlayerInputs, ((MatchInput)obj).PlayerInputs);
  }

  public override int GetHashCode() => ArrayUtil.GetOrderedHash(PlayerInputs);

  public override string ToString() => $"MatchInput({PlayerInputs?.Length ?? 0})";

  public void Serialize(NetworkWriter writer) {
    if (PlayerInputs == null) return;
    for (var i = 0; i < PlayerInputs.Length; i++) {
      PlayerInputs[i].Serialize(writer);
    }
  }

  public static MatchInput Deserialize(NetworkReader reader, int players) {
    var inputs = new PlayerInput[players];
    for (var i = 0; i < inputs.Length; i++) {
      inputs[i] = PlayerInput.Deserialize(reader);
    }
    return new MatchInput { PlayerInputs = inputs };
  }

}

public class MatchInputContext {

  public PlayerInputContext[] PlayerInputs;

  public MatchInputContext(MatchConfig config) : this(new MatchInput(config)) {
  }

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
