using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEngine.Networking;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct MatchStartMessage : INetworkSerializable {
  public MatchConfig MatchConfig;

  public void Serialize(ref Serializer serializer) => serializer.Write(MatchConfig);
  public void Deserialize(ref Deserializer deserializer) => MatchConfig = deserializer.Read<MatchConfig>();
}

}