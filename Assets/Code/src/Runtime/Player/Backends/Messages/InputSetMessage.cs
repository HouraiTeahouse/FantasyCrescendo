using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class InputSetMessage : MessageBase {

  public uint StartTimestamp;
  public MatchInput[] Inputs;

  public override void Serialize(NetworkWriter writer) {
    writer.WritePackedUInt32(StartTimestamp);
    writer.WritePackedUInt32((uint)Inputs.Length);
    if (Inputs.Length == 0) return;
    writer.WritePackedUInt32((uint)Inputs[0].PlayerCount);
    for (int i = 0; i < Inputs.Length; i++) {
      Inputs[i].Serialize(writer);
    }
  }

  public override void Deserialize(NetworkReader reader) {
    StartTimestamp = reader.ReadPackedUInt32();
    var length = reader.ReadPackedUInt32();
    if (length <= 0) return;
    var playerCount = reader.ReadPackedUInt32();
    Inputs = new MatchInput[length];
    for (int i = 0; i < Inputs.Length; i++) {
      Inputs[i] = MatchInput.Deserialize(reader, (int)playerCount);
    }
  }

}

}
