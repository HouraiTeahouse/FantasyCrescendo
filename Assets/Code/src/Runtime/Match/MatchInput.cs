using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Linq;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Mask = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public unsafe struct MatchInput : IMergable<MatchInput> {

  static MatchInput() {
    Assert.IsTrue(UnsafeUtility.IsBlittable<PlayerInput>());
    Assert.IsTrue(UnsafeUtility.SizeOf<PlayerInput>() <= kPlayerInputSize);
    Assert.IsTrue(sizeof(Mask) * 8 >= kMaxSupportedPlayers);
  }

  public enum MergeStrategy {
    FullMerge,        // Completely copies over the new inputs
    KeepValidity      // Copies over everything except for the validity of the new inputs
  }

  const int kPlayerInputSize = 5;
  public const int kMaxSupportedPlayers = (int)GameMode.GlobalMaxPlayers;
  Mask AllValid => BitUtil.AllBits(PlayerCount); 

  public int PlayerCount { get; }
  public Mask ValidMask;
  fixed byte _inputs[kPlayerInputSize * kMaxSupportedPlayers];

  /// <summary>
  /// Gets the input for the given player.
  /// </summary>
  /// <param name="index">the index of the player's inputs.</param>
  public unsafe ref PlayerInput this[int idx] {
    get {
      fixed (byte* inputPtr = _inputs) {
        return ref ((PlayerInput*)inputPtr)[idx];
      }
    }
  }

  public MatchInput(int playerCount) {
    ValidMask = (Mask)0;
    PlayerCount = Mathf.Min(playerCount, kMaxSupportedPlayers);
  }

  public MatchInput(MatchConfig config) : this(config.PlayerCount) {}
  public bool IsValid => (ValidMask & AllValid) != 0;

  MatchInput IMergable<MatchInput>.MergeWith(MatchInput other) => MergeWith(other);
  public unsafe MatchInput MergeWith(MatchInput other, 
                                     MergeStrategy strategy = MergeStrategy.FullMerge) {
    Assert.IsTrue(PlayerCount >= other.PlayerCount);
    var newInput = this;
    fixed (byte* selfData = _inputs) {
      var newInputs = (PlayerInput*)newInput._inputs;
      var selfInputs = (PlayerInput*)selfData;
      var otherInputs = (PlayerInput*)other._inputs;
      switch (strategy) {
        case MergeStrategy.FullMerge:
          newInput.ValidMask = (byte)(ValidMask | other.ValidMask);
          for (int i = 0; i < PlayerCount; i++) {
            if (!IsPlayerValid(i) && other.IsPlayerValid(i)) {
              newInputs[i] = otherInputs[i];
            } 
          }
          break;
        case MergeStrategy.KeepValidity:
          for (int i = 0; i < PlayerCount; i++) {
            if (!IsPlayerValid(i)) {
              newInputs[i] = otherInputs[i];
            }
          }
          break;
      }
    }
    return newInput;
  }

  public bool IsPlayerValid(int playerId) => BitUtil.GetBit(ValidMask, playerId);
  public void Predict() => ValidMask = AllValid;
  internal void Reset() => ValidMask = 0;

  public static bool operator ==(MatchInput a, MatchInput b) {
    if (a.PlayerCount != b.PlayerCount) return false;
    bool equal = true;
    for (var i = 0; i < a.PlayerCount; i++) {
      equal &= a._inputs[i] == b._inputs[i];
    }
    return equal;
  }

  public static bool operator !=(MatchInput a, MatchInput b) => !(a == b);

  public override int GetHashCode() {
    unchecked {
      var hash = PlayerCount;
      for (var i = 0; i < PlayerCount; i++) {
        hash = (hash << 3) + _inputs[i].GetHashCode();
      }
      return hash;
    }
  }

  public override string ToString() => $"MatchInput({PlayerCount}, {GetHashCode():X})";

}

public class MatchInputContext {

  MatchInput before;
  MatchInput current;

  public MatchInputContext(MatchConfig config) : this(new MatchInput(config)) {
  }

  public MatchInputContext(MatchInput input) {
    Reset(input);
    Predict();
  }

  public PlayerInputContext this[int idx] => 
    new PlayerInputContext(ref before[idx], ref current[idx]);

  public bool IsValid => before.IsValid && current.IsValid;
  public int PlayerCount => current.PlayerCount;

  public void Predict() {
    before.Predict();
    current.Predict();
  }

  public void Reset(MatchInput input) => current = input;

  public void Reset(MatchInput previous, MatchInput current) {
    Reset(previous);
    Update(current);
  }

  public void Update(MatchInput next) {
    Assert.AreEqual(current.PlayerCount, next.PlayerCount);
    before = current;
    current = next;
  }

}

}
