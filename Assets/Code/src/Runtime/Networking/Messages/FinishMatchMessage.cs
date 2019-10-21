using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct MatchFinishMessage : INetworkSerializable {

  public MatchResult MatchResult;

  public void Serialize(ref Serializer serializer) => serializer.Write(MatchResult);
  public void Deserialize(ref Deserializer deserializer) => MatchResult = deserializer.Read<MatchResult>();

}

}