using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Mask = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class InputSetMessage : MessageBase, IDisposable {

  public uint StartTimestamp;
  public uint InputCount;
  // TODO(james7132): Derive Player Count from Valid Mask
  public byte ValidMask;
  public MatchInput[] Inputs;

  public override void Serialize(NetworkWriter writer) {
    Assert.IsTrue(InputCount <= Inputs.Length);
    writer.WritePackedUInt32(InputCount);             // 1-4 bytes
    if (InputCount <= 0) return;
    var firstInput = Inputs[0];
    var playerCount = (byte)firstInput.PlayerCount;
    Assert.IsTrue(playerCount <= sizeof(Mask) * 8);
    writer.Write(ValidMask);                          // 1 byte
    writer.WritePackedUInt32(StartTimestamp);         // 1-4 bytes
    writer.Write(playerCount);                        // 1 byte
    for (int i = 0; i < InputCount; i++) {            // 1-5 * playerCount * Inputs.Length bytes
      Inputs[i].Serialize(writer, ValidMask);         // (Only valid inputs)
    }
  }

  public override void Deserialize(NetworkReader reader) {
    InputCount = reader.ReadPackedUInt32();
    if (InputCount <= 0) return;
    ValidMask = reader.ReadByte();
    StartTimestamp = reader.ReadPackedUInt32();
    byte playerCount = reader.ReadByte();
    Inputs = ArrayPool<MatchInput>.Shared.Rent((int)InputCount);
    for (int i = 0; i < InputCount; i++) {
      Inputs[i] = MatchInput.Deserialize(reader, (int)playerCount, ValidMask);
    }
  }

  public void Dispose() {
    if (Inputs == null) return;
    ArrayPool<MatchInput>.Shared.Return(Inputs);
    Inputs = null;
  }

}

}
