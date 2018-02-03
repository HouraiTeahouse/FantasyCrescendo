using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class ServerStateMessage : MessageBase {

  public uint Timestamp;
  public MatchState State;

  public override void Serialize(NetworkWriter writer) {
    writer.WritePackedUInt32(Timestamp);
    writer.WritePackedUInt32((uint)State.PlayerCount);
    for (uint i = 0; i < State.PlayerCount; i++) {
      State.GetPlayerState(i).Serialize(writer);
    }
  }

  public override void Deserialize(NetworkReader reader) {
    Timestamp = reader.ReadPackedUInt32();
    var count = reader.ReadPackedUInt32();
    State = new MatchState((int)count);
    for (uint i = 0; i < count; i++) {
      var state = new PlayerState();
      state.Deserialize(reader);
      State.SetPlayerState(i, state);
    }
  }

}

}
