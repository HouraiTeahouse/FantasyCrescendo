using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Mask = System.Byte;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct InputSetMessage : INetworkSerializable, IDisposable {

  public uint StartTimestamp;
  public uint InputCount;
  public MatchInput[] Inputs;

  public ArraySegment<MatchInput> AsArraySegment() => new ArraySegment<MatchInput>(Inputs, 0, (int)InputCount);

  public void Serialize(ref Serializer serializer) {
    Assert.IsTrue(InputCount <= Inputs.Length);
    serializer.Write(InputCount);                           // 1-4 bytes
    if (InputCount <= 0) return;
    var firstInput = Inputs[0];
    var playerCount = (byte)firstInput.PlayerCount;

    var mask = GetValidMask();
    // The number of players is encoded as the N + 1 bit in the ValidMask
    // The number of bits represents the number of players stored.
    // As the size of the mask is one byte, the maximum supported players
    // is (8 bits - 1 for count) => 7 players.
    Assert.IsTrue(playerCount < sizeof(Mask) * 8);
    mask &= (byte)((1 << playerCount + 1) - 1);        // Disable all bits higher than N + 1
    mask |= (byte)(1 << playerCount);                  // Set the count bit to 1.

    serializer.Write(mask);                            // 1 byte
    serializer.Write(StartTimestamp);                       // 1-4 bytes
    for (var i = 0; i < playerCount; i++) {
      if (!BitUtil.GetBit(mask, i)) continue;
      PlayerInput? lastInput= null;
      for (int j = 0; j < InputCount; j++) {                // 1-5 * playerCount * Inputs.Length bytes
        var currentInput = Inputs[j][i];                    // (Only valid inputs)
        currentInput.Serialize(ref serializer, lastInput);
        lastInput = currentInput;
      }
    }
  }

  public void Deserialize(ref Deserializer deserializer) {
    InputCount = deserializer.ReadUInt32();
    if (InputCount <= 0) return;
    byte mask = deserializer.ReadByte();
    int playerCount = BitUtil.GetBitCount(mask);
    StartTimestamp = deserializer.ReadUInt32();
    Inputs = ArrayPool<MatchInput>.Shared.Rent((int)InputCount);
    for (var i = 0; i < InputCount; i++) {
      Inputs[i] = new MatchInput(playerCount);
      Inputs[i].ValidMask = mask;
    }
    for (var i = 0; i < playerCount; i++) {
      if (!BitUtil.GetBit(mask, i)) continue;
      PlayerInput? lastInput = null;
      for (int j = 0; j < InputCount; j++) {
        PlayerInput.Deserialize(ref deserializer, ref Inputs[j][i], 
                                ref lastInput);
        lastInput = Inputs[j][i];
      }
    }
  }

  public void Dispose() {
    if (Inputs == null) return;
    ArrayPool<MatchInput>.Shared.Return(Inputs);
    Inputs = null;
  }

  byte GetValidMask() {
    if (Inputs.Length <= 0) return 0;
    byte mask = 255;
    for (var i = 0; i < Inputs.Length; i++) {
      mask &= Inputs[i].ValidMask;
    }
    return mask;
  }

}

}
