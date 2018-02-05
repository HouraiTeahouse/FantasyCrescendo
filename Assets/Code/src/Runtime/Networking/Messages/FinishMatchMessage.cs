using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class MatchFinishMessage : INetworkSerializable {
  public MatchResult MatchResult;

  public void Serialize(Serializer serializer) => serializer.Write(MatchResult);
  public void Deserialize(Deserializer deserializer) => MatchResult = deserializer.Read<MatchResult>();

}

}