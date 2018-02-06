using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct MatchStartMessage : INetworkSerializable {
  public MatchConfig MatchConfig;

  public void Serialize(Serializer serializer) => serializer.Write(MatchConfig);
  public void Deserialize(Deserializer deserializer) => MatchConfig = deserializer.Read<MatchConfig>();
}

}