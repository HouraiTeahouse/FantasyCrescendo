using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public class UNETServer : INetworkServer {

  public UNETServer() {
    NetworkServer.RegisterHandler(MessageCode.UpdateInput, OnReceivedClientInput);
  }

  public int ClientCount {
    get { return NetworkServer.connections.Count; }
  }

  public event Action<int, uint, IEnumerable<MatchInput>> ReceivedInputs;

  public void BroadcastInput(uint startTimestamp, IEnumerable<MatchInput> input) {
    NetworkServer.SendUnreliableToAll(MessageCode.UpdateInput, new InputSetMessage {
        StartTimestamp = startTimestamp,
        Inputs = input.ToArray()
    });
  }

  public void BroadcastState(uint timestamp, MatchState state) {
    NetworkServer.SendUnreliableToAll(MessageCode.UpdateState, new ServerStateMessage {
        Timestamp = timestamp,
        State = state
    });
  }

  void OnReceivedClientInput(NetworkMessage message) {
    if (ReceivedInputs == null) {
      return;
    }
    var inputSet = message.ReadMessage<InputSetMessage>();
    ReceivedInputs(message.conn.connectionId,
                   inputSet.StartTimestamp,
                   inputSet.Inputs);
  }

  public void Dispose() {
    NetworkServer.UnregisterHandler(MessageCode.UpdateInput);
  }

}

}
