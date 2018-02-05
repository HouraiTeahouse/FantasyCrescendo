using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class ServerStateMessage : INetworkSerializable {

  public uint Timestamp;
  public MatchState State;

  public void Serialize(Serializer serializer) {
    serializer.WritePackedUInt32(Timestamp);
    serializer.Write(State);
  }

  public void Deserialize(Deserializer deserializer) {
    Timestamp = deserializer.ReadPackedUInt32();
    State = deserializer.Read<MatchState>();
  }

}

}
