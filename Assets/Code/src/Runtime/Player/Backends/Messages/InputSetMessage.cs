using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Mask = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class InputSetMessage : MessageBase {

  public uint StartTimestamp;
  public MatchInput[] Inputs;

  public override void Serialize(NetworkWriter writer) {
    writer.WritePackedUInt32(StartTimestamp);         // 1-4 bytes
    writer.WritePackedUInt32((uint)Inputs.Length);    // 1-4 bytes
    if (Inputs.Length == 0) return;
    var firstInput = Inputs[0];
    var playerCount = (byte)firstInput.PlayerCount;
    var mask = firstInput.CreateValidMask();
    Assert.IsTrue(playerCount <= sizeof(Mask) * 8);
    writer.Write(playerCount);                        // 1 byte
    writer.Write(mask);                               // 1 byte
    for (int i = 0; i < Inputs.Length; i++) {         // 1-5 * playerCount * Inputs.Length bytes
      Inputs[i].Serialize(writer, mask);              // (Only valid inputs)
    }
  }

  public override void Deserialize(NetworkReader reader) {
    StartTimestamp = reader.ReadPackedUInt32();
    var length = reader.ReadPackedUInt32();
    if (length <= 0) return;
    byte playerCount = reader.ReadByte();
    byte mask = reader.ReadByte();
    Inputs = new MatchInput[length];
    for (int i = 0; i < Inputs.Length; i++) {
      Inputs[i] = MatchInput.Deserialize(reader, (int)playerCount, mask);
    }
  }

}

}
