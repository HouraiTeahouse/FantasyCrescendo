using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Mask = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public struct MatchInput : IMergable<MatchInput>, IDisposable {

  public const int kMaxSupportedPlayers = sizeof(Mask) * 8;
  public int PlayerCount { get; }

  /// <summary> The collection of all player inputs. </summary>
  /// <remarks> This array may be larger than PlayerCount. </remarks>
  public PlayerInput[] PlayerInputs;

  public MatchInput(int playerCount) {
    PlayerCount = playerCount;
    PlayerInputs = ArrayPool<PlayerInput>.Shared.Rent(PlayerCount);
    Array.Clear(PlayerInputs, 0, PlayerCount);
  }

  public MatchInput(MatchConfig config) : this(config.PlayerConfigs.Length) {}

  MatchInput(MatchInput input) {
    PlayerCount = input.PlayerCount;
    PlayerInputs = ArrayPool<PlayerInput>.Shared.Rent(PlayerCount);
    Array.Copy(input.PlayerInputs, 0, PlayerInputs, 0, PlayerCount);
  }

  public void Dispose() {
    if (PlayerInputs == null) return;
    ArrayPool<PlayerInput>.Shared.Return(PlayerInputs);
    PlayerInputs = null;
  }

  public bool IsValid => PlayerInputs.IsAllValid();

  public void Predict(MatchInput baseInput) => ForceValid(PlayerInputs, true);

  public void MergeWith(MatchInput otherInput) {
    Assert.IsTrue(PlayerCount >= otherInput.PlayerInputs.Length);
    for (int i = 0; i < PlayerCount; i++) {
      if (!PlayerInputs[i].IsValid && otherInput.PlayerInputs[i].IsValid) {
        PlayerInputs[i] = otherInput.PlayerInputs[i];
      }
    }
  }

  internal void Reset() => ForceValid(PlayerInputs, false);

  static void ForceValid(PlayerInput[] inputs, bool valid) {
    for (int i = 0; i < inputs.Length; i++) {
      inputs[i].IsValid = valid;
    }
  }

  public MatchInput Clone() => new MatchInput(this);

  public override bool Equals(object obj) {
    if (!(obj is MatchInput)) return false;
    var other = (MatchInput)obj;
    if (PlayerInputs == null || other.PlayerInputs == null) return false;
    if (PlayerCount != other.PlayerCount) return false;
    bool equal = true;
    for (var i = 0; i < PlayerCount; i++) {
      equal &= PlayerInputs[i].Equals(other.PlayerInputs[i]);
    }
    return equal;
  }

  public override int GetHashCode() => ArrayUtil.GetOrderedHash(PlayerInputs);

  public override string ToString() => $"MatchInput({PlayerCount})";

  public Mask CreateValidMask() {
    Assert.IsTrue(PlayerCount <= kMaxSupportedPlayers);
    Mask mask = 0;
    for (var i = 0; i < PlayerCount; i++) {
      mask |= (Mask)(PlayerInputs[i].IsValid ? 1 << i : 0);
    }
    return mask;
  }

  public void Serialize(NetworkWriter writer, Mask mask) {
    if (PlayerInputs == null) return;
    Assert.IsTrue(PlayerCount <= kMaxSupportedPlayers);
    for (var i = 0; i < PlayerCount; i++) {
      if ((mask & (1 << i)) == 0) continue;
      Assert.IsTrue(PlayerInputs[i].IsValid);
      PlayerInputs[i].Serialize(writer);
    }
  }

  public static MatchInput Deserialize(NetworkReader reader, int players, Mask mask) {
    Assert.IsTrue(players <= kMaxSupportedPlayers);
    var input = new MatchInput(players);
    for (var i = 0; i < input.PlayerCount; i++) {
      if ((mask & (1 << i)) == 0) continue;
      input.PlayerInputs[i] = PlayerInput.Deserialize(reader);
    }
    return input;
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
    Assert.AreEqual(input.PlayerCount, input.PlayerInputs.Length);
    for (int i = 0; i < input.PlayerInputs.Length; i++) {
      PlayerInputs[i].Update(input.PlayerInputs[i]);
    }
  }

}

}
