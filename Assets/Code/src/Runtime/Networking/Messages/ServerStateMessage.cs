using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct ServerStateMessage : INetworkSerializable {

  public uint Timestamp;
  public MatchState State;
  public MatchInput? LatestInput;

  public void Serialize(ref Serializer serializer) {
    serializer.Write(Timestamp);
    serializer.Write(State);
    if (LatestInput == null) {
      serializer.Write((byte)0);
    } else {
      var input = LatestInput.Value;
      var count = input.PlayerCount;
      serializer.Write((byte)count);
      for (var i = 0; i < count; i++) {
        input[i].Serialize(ref serializer);
      }
    }
  }

  public void Deserialize(ref Deserializer deserializer) {
    Timestamp = deserializer.ReadUInt32();
    State = deserializer.ReadMessage<MatchState>();
    var count = deserializer.ReadByte();
    if (count == 0) {
      LatestInput = null;
    } else {
      var input = new MatchInput(count);
      for (var i = 0; i < count; i++) {
        PlayerInput.Deserialize(ref deserializer, ref input[i]);
      }
      LatestInput = input;
    }
  }

}

}
