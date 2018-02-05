using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class ServerUpdateConfigMessage : INetworkSerializable {
  public MatchConfig MatchConfig;

  public void Serialize(Serializer serializer) => serializer.Write(MatchConfig);
  public void Deserialize(Deserializer deserializer) => MatchConfig = deserializer.Read<MatchConfig>();
}

public class ClientUpdateConfigMessage : INetworkSerializable {
  public PlayerConfig PlayerConfig;

  public void Serialize(Serializer serializer) => serializer.Write(PlayerConfig);
  public void Deserialize(Deserializer deserializer) => PlayerConfig = deserializer.Read<PlayerConfig>();
}

}
