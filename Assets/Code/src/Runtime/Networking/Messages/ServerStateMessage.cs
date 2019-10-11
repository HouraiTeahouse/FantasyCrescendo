using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct ServerStateMessage : INetworkSerializable {

  public uint Timestamp;
  public MatchState State;
  public MatchInput? LatestInput;

  public void Serialize(Serializer serializer) {
    serializer.Write(Timestamp);
    serializer.Write(State);
    if (LatestInput == null) {
      serializer.Write((byte)0);
    } else {
      var input = LatestInput.Value;
      var count = input.PlayerCount;
      serializer.Write((byte)count);
      for (var i = 0; i < count; i++) {
        input[i].Serialize(serializer);
      }
    }
  }

  public void Deserialize(Deserializer deserializer) {
    Timestamp = deserializer.ReadUInt32();
    State = deserializer.ReadMessage<MatchState>();
    var count = deserializer.ReadByte();
    if (count == 0) {
      LatestInput = null;
    } else {
      var input = new MatchInput(count);
      for (var i = 0; i < count; i++) {
        PlayerInput.Deserialize(deserializer, ref input[i]);
      }
      LatestInput = input;
    }
  }

}

}
