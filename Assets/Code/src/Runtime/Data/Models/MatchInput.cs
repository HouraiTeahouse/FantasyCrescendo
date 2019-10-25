using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Linq;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using HouraiTeahouse.Backroll;

namespace HouraiTeahouse.FantasyCrescendo {

public unsafe struct MatchInput {

  static MatchInput() {
    Assert.IsTrue(UnsafeUtility.IsBlittable<PlayerInput>());
    Assert.IsTrue(UnsafeUtility.SizeOf<PlayerInput>() == kPlayerInputSize);
  }

  const int kPlayerInputSize = 5;
  fixed byte _inputs[kPlayerInputSize * BackrollConstants.kMaxPlayers];

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

}

public class MatchInputContext {

  MatchInput before;
  MatchInput current;

  public PlayerInputContext this[int idx] => 
    new PlayerInputContext(ref before[idx], ref current[idx]);

  public void Reset(MatchInput input) => current = input;

  public void Reset(MatchInput previous, MatchInput current) {
    Reset(previous);
    Update(current);
  }

  public void Update(MatchInput next) {
    before = current;
    current = next;
  }

}

}
