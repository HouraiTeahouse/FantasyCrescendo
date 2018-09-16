using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Mask = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public enum MatchInputMergeStrategy {
  FullMerge,        // Completely copies over the new inputs
  KeepValidity      // Copies over everything except for the validity of the new inputs
}

public unsafe struct MatchInput : IMergable<MatchInput> {

  const int kPlayerInputSize = 5;
  public const int kMaxSupportedPlayers = (int)GameMode.GlobalMaxPlayers;
  public const Mask AllValid = (byte)((1 << kMaxSupportedPlayers) - 1);

  public int PlayerCount { get; }

  fixed byte Data[kMaxSupportedPlayers * kPlayerInputSize];

  /// <summary>
  /// Gets the input for the player at a given frame.
  /// </summary>
  /// <remarks>
  /// This indexer does **not** do any bounds checking. Be sure to do a
  /// check against 0 and PlayerCount before using it.
  /// </remarks>
  /// <param name="index">the index of the player's inputs.</param>
  public unsafe ref PlayerInput this[int index] {
    get { 
      fixed (byte* inputPtr = Data) {
        return ref ((PlayerInput*)inputPtr)[index];
      }
    }
  }

  public MatchInput(int playerCount) {
    PlayerCount = Mathf.Min(playerCount, kMaxSupportedPlayers);
  }

  public MatchInput(MatchConfig config) : this(config.PlayerConfigs.Length) {}

  public bool IsValid {
    get { 
      fixed (byte* data = Data) {
        bool isValid = true;
        var inputs = (PlayerInput*)data;
        for (int i = 0; i < PlayerCount; i++) {
          isValid &= inputs[i].IsValid;
        }
        return isValid;
      }
    }
  }

  public void Predict() => ForceValid(true);

  public unsafe MatchInput MergeWith(MatchInput other) => MergeWith(other, MatchInputMergeStrategy.FullMerge);

  public unsafe MatchInput MergeWith(MatchInput other, MatchInputMergeStrategy strategy) {
    Assert.IsTrue(PlayerCount >= other.PlayerCount);
    var newInput = this;
    fixed (byte* selfData = Data) {
      var newInputs = (PlayerInput*)newInput.Data;
      var selfInputs = (PlayerInput*)selfData;
      var otherInputs = (PlayerInput*)other.Data;
      switch (strategy) {
        case MatchInputMergeStrategy.FullMerge:
          for (int i = 0; i < PlayerCount; i++) {
            if (!selfInputs[i].IsValid && otherInputs[i].IsValid) {
              newInputs[i] = otherInputs[i];
            } else {
              newInputs[i] = selfInputs[i];
            }
          }
          break;
        case MatchInputMergeStrategy.KeepValidity:
          for (int i = 0; i < PlayerCount; i++) {
            if (!selfInputs[i].IsValid) {
              newInputs[i] = otherInputs[i];
              newInputs[i].IsValid = false;
            } else {
              newInputs[i] = selfInputs[i];
            }
          }
          break;
      }
    }
    return newInput;
  }

  internal void Reset() => ForceValid(false);

  unsafe void ForceValid(bool valid) {
    fixed (byte* data = Data) {
      var inputs = (PlayerInput*)data;
      for (int i = 0; i < PlayerCount; i++) {
        inputs[i].IsValid = valid;
      }
    }
  }

  public override bool Equals(object obj) {
    if (!(obj is MatchInput)) return false;
    var other = (MatchInput)obj;
    var maxPlayerCount = Mathf.Max(PlayerCount, other.PlayerCount);
    bool equal = true;
    for (var i = 0; i < maxPlayerCount; i++) {
      if (i < PlayerCount && i < other.PlayerCount) {
        equal &= this[i].Equals(other[i]);
      } else if (i < PlayerCount) {
        if (this[i].IsValid) return false;
      } else if (i < other.PlayerCount) {
        if (other[i].IsValid) return false;
      } else {
        break;
      }
      // Debug.Log($"{i} {PlayerCount} {other.PlayerCount} {this[i]} {other[i]} {equal}");
    }
    return equal;
  }

  public override int GetHashCode() {
    unchecked {
      var hash = PlayerCount;
      for (var i = 0; i < PlayerCount; i++) {
        hash += this[i].GetHashCode();
      }
      return hash;
    }
  }

  public override string ToString() => $"MatchInput({PlayerCount}, {GetHashCode():X})";

  public Mask CreateValidMask() {
    Assert.IsTrue(PlayerCount <= kMaxSupportedPlayers);
    Mask mask = 0;
    fixed (byte* data = Data) {
      var inputs = (PlayerInput*)data;
      for (int i = 0; i < PlayerCount; i++) {
        mask |= (Mask)(inputs[i].IsValid ? (1 << i) : 0);
      }
    }
    return mask;
  }

}

public class MatchInputContext {

  public PlayerInputContext[] PlayerInputs;

  public MatchInputContext(MatchConfig config) : this(new MatchInput(config)) {
  }

  public MatchInputContext(MatchInput input) {
    Reset(input);
    Predict();
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

  public void Predict() {
    for (int i = 0; i < PlayerInputs.Length; i++) {
      PlayerInputs[i].ForceValid(true);
    }
  }

  public void Reset(MatchInput current) {
    if (PlayerInputs?.Length != current.PlayerCount) {
      PlayerInputs = new PlayerInputContext[current.PlayerCount];
    }
    for (int i = 0; i < PlayerInputs.Length; i++) {
      PlayerInputs[i] = new PlayerInputContext {
        Current = current[i]
      };
    }
  }

  public void Reset(MatchInput previous, MatchInput current) {
    Reset(previous);
    Update(current);
  }

  public void Update(MatchInput input) {
    Assert.AreEqual(PlayerInputs.Length, input.PlayerCount);
    for (int i = 0; i < input.PlayerCount; i++) {
      PlayerInputs[i].Update(input[i]);
    }
  }

}

}
