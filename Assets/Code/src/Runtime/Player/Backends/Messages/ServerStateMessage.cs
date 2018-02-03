using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class ServerStateMessage : MessageBase {

  public uint Timestamp;
  public MatchState State;

  public override void Serialize(NetworkWriter writer) {
    writer.WritePackedUInt32(Timestamp);
    State.Serialize(writer);
  }

  public override void Deserialize(NetworkReader reader) {
    Timestamp = reader.ReadPackedUInt32();
    if (State == null) {
      State = new MatchState(0);
    }
    State.Deserialize(reader);
  }

}

}
