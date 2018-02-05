using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Mask = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public struct MatchInput : IMergable<MatchInput>, IDisposable {

  public const Mask AllValid = (byte)255;

  public const int kMaxSupportedPlayers = sizeof(Mask) * 8;
  public int PlayerCount { get; }

  /// <summary> The collection of all player inputs. </summary>
  /// <remarks> This array may be larger than PlayerCount. </remarks>
  public PlayerInput[] PlayerInputs;

  public MatchInput(int playerCount) {
    PlayerCount = playerCount;
    Assert.IsTrue(PlayerCount <= kMaxSupportedPlayers, $"PlayerCount not in range: {PlayerCount}");
    PlayerInputs = ArrayPool<PlayerInput>.Shared.Rent(PlayerCount);
    Array.Clear(PlayerInputs, 0, PlayerCount);
  }

  public MatchInput(MatchConfig config) : this(config.PlayerConfigs.Length) {}

  MatchInput(MatchInput input) : this(input.PlayerCount) {
    Array.Copy(input.PlayerInputs, 0, PlayerInputs, 0, PlayerCount);
  }

  public void Dispose() {
    if (PlayerInputs == null) return;
    ArrayPool<PlayerInput>.Shared.Return(PlayerInputs);
    PlayerInputs = null;
  }

  public bool IsValid {
    get { 
      bool isValid = true;
      for (var i = 0; i < PlayerCount; i++) {
        isValid &= PlayerInputs[i].IsValid;
      }
      return isValid;
    }
  }

  public void Predict() => ForceValid(PlayerInputs, true);

  public void MergeWith(MatchInput otherInput) {
    Assert.IsTrue(PlayerCount >= otherInput.PlayerCount);
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
    var maxPlayerCount = Mathf.Max(PlayerCount, other.PlayerCount);
    bool equal = true;
    for (var i = 0; i < maxPlayerCount; i++) {
      Debug.Log($"{i} {PlayerCount} {other.PlayerCount} {PlayerInputs[i]} {other.PlayerInputs[i]}");
      if (i < PlayerCount && i < other.PlayerCount) {
        equal &= PlayerInputs[i].Equals(other.PlayerInputs[i]);
      } else if (i < PlayerCount) {
        if (PlayerInputs[i].IsValid) return false;
      } else if (i < other.PlayerCount) {
        if (other.PlayerInputs[i].IsValid) return false;
      } else {
        break;
      }
    }
    return equal;
  }

  public override int GetHashCode() => ArrayUtil.GetOrderedHash(PlayerInputs);

  public override string ToString() => $"MatchInput({PlayerCount}, {GetHashCode():X})";

  public Mask CreateValidMask() {
    Assert.IsTrue(PlayerCount <= kMaxSupportedPlayers);
    Mask mask = 0;
    for (var i = 0; i < PlayerCount; i++) {
      mask |= (Mask)(PlayerInputs[i].IsValid ? (1 << i) : 0);
    }
    return mask;
  }

  public void Serialize(NetworkWriter writer, Mask mask) {
    if (PlayerInputs == null) return;
    for (var i = 0; i < PlayerCount; i++) {
      if ((mask & (1 << i)) == 0) continue;
      PlayerInputs[i].Serialize(writer);
    }
  }

  public static MatchInput Deserialize(NetworkReader reader, int players, Mask mask) {
    Assert.IsTrue(players <= kMaxSupportedPlayers);
    var input = new MatchInput(players);
    for (var i = 0; i < input.PlayerCount; i++) {
      if ((mask & (1 << i)) != 0) {
        input.PlayerInputs[i] = PlayerInput.Deserialize(reader);
      } else {
        input.PlayerInputs[i] = new PlayerInput { IsValid = false };
      }
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

  public void Predict() {
    for (int i = 0; i < PlayerInputs.Length; i++) {
      PlayerInputs[i].Current.IsValid = true;
      PlayerInputs[i].Previous.IsValid = true;
    }
  }

  public void Reset(MatchInput current) {
    if (PlayerInputs?.Length != current.PlayerCount) {
      PlayerInputs = new PlayerInputContext[current.PlayerCount];
    }
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
    Assert.AreEqual(PlayerInputs.Length, input.PlayerCount);
    for (int i = 0; i < input.PlayerCount; i++) {
      PlayerInputs[i].Update(input.PlayerInputs[i]);
    }
  }

}

}
