using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct ServerUpdateConfigMessage : INetworkSerializable {
  public MatchConfig MatchConfig;

  public void Serialize(ref Serializer serializer) => serializer.Write(MatchConfig);
  public void Deserialize(ref Deserializer deserializer) => MatchConfig = deserializer.Read<MatchConfig>();
}

public struct ClientUpdateConfigMessage : INetworkSerializable {
  public PlayerConfig PlayerConfig;

  public void Serialize(ref Serializer serializer) => serializer.Write(PlayerConfig);
  public void Deserialize(ref Deserializer deserializer) => PlayerConfig = deserializer.Read<PlayerConfig>();
}

}
