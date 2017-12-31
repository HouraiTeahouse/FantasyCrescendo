using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public class UNETServer : INetworkServer {

  public UNETServer() {
    NetworkServer.RegisterHandler(MessageCode.UpdateInput, OnRecievedClientInput);
  }

  public int ClientCount {
    get { return NetworkServer.connections.Count; }
  }

  public event Action<int, uint, IEnumerable<GameInput>> RecievedInputs;

  public void BroadcastInput(uint startTimestamp, IEnumerable<GameInput> input) {
    NetworkServer.SendUnreliableToAll(MessageCode.UpdateInput, new InputSetMessage {
        StartTimestamp = startTimestamp,
        Inputs = input.ToArray()
    });
  }

  public void BroadcastState(uint timestamp, GameState state) {
    NetworkServer.SendUnreliableToAll(MessageCode.UpdateState, new ServerStateMessage {
        Timestamp = timestamp,
        State = state
    });
  }

  void OnRecievedClientInput(NetworkMessage message) {
    if (RecievedInputs == null) {
      return;
    }
    var inputSet = message.ReadMessage<InputSetMessage>();
    RecievedInputs(message.conn.connectionId,
                   inputSet.StartTimestamp,
                   inputSet.Inputs);
  }

  public void Dispose() {
    NetworkServer.UnregisterHandler(MessageCode.UpdateInput);
  }

}

}
