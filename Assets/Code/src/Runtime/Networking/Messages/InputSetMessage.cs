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
using HouraiTeahouse.Backroll;

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

    byte mask = 0;

    serializer.Write(mask);                            // 1 byte
    serializer.Write(StartTimestamp);                       // 1-4 bytes
    for (var i = 0; i < BackrollConstants.kMaxPlayers; i++) {
      if (!BitUtil.GetBit(mask, i)) continue;
      PlayerInput? lastInput= null;
      for (int j = 0; j < InputCount; j++) {                // 1-5 * playerCount * Inputs.Length bytes
        var currentInput = Inputs[j][i];                    // (Only valid inputs)
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
    for (var i = 0; i < playerCount; i++) {
      if (!BitUtil.GetBit(mask, i)) continue;
      PlayerInput? lastInput = null;
      for (int j = 0; j < InputCount; j++) {
        lastInput = Inputs[j][i];
      }
    }
  }

  public void Dispose() {
    if (Inputs == null) return;
    ArrayPool<MatchInput>.Shared.Return(Inputs);
    Inputs = null;
  }

}

}
