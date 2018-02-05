using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class PeerReadyMessage : INetworkSerializable {
  public bool IsReady;

  public void Serialize(Serializer serializer) => serializer.Write(IsReady);
  public void Deserialize(Deserializer deserializer) => IsReady = deserializer.ReadBoolean();

}

}

