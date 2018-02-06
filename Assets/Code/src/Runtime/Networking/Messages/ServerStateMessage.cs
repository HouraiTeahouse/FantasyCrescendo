using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct ServerStateMessage : INetworkSerializable {

  public uint Timestamp;
  public MatchState State;

  public void Serialize(Serializer serializer) {
    serializer.Write(Timestamp);
    serializer.Write(State);
  }

  public void Deserialize(Deserializer deserializer) {
    Timestamp = deserializer.ReadUInt32();
    State = deserializer.ReadMessage<MatchState>();
  }

}

}
