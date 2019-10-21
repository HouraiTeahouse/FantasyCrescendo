using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct PeerReadyMessage : INetworkSerializable {

  public bool IsReady;

  public void Serialize(ref Serializer serializer) => serializer.Write(IsReady);
  public void Deserialize(ref Deserializer deserializer) => IsReady = deserializer.ReadBoolean();

}

}

