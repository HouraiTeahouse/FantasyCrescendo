using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct PeerReadyMessage : INetworkSerializable {

  public bool IsReady;

  public void Serialize(Serializer serializer) => serializer.Write(IsReady);
  public void Deserialize(Deserializer deserializer) => IsReady = deserializer.ReadBoolean();

}

}

