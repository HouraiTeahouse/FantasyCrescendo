using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkGameServer : INetworkServer {

  readonly INetworkInterface NetworkInterface;

  public NetworkGameServer(INetworkInterface networkInterface, NetworkServerConfig config) {
    NetworkInterface = networkInterface;

    NetworkInterface.MessageHandlers.RegisterHandler(MessageCodes.UpdateInput, OnReceivedClientInput);
  }

  public int ClientCount => NetworkServer.connections.Count; 

  public event Action<uint, uint, IEnumerable<MatchInput>> ReceivedInputs;

  public void BroadcastInput(uint startTimestamp, IEnumerable<MatchInput> input) {
    NetworkInterface.Connections.SendToAll(MessageCodes.UpdateInput, new InputSetMessage {
      StartTimestamp = startTimestamp,
      Inputs = input.ToArray()
    }, NetworkReliablity.Unreliable);
  }

  public void BroadcastState(uint timestamp, MatchState state) {
    NetworkInterface.Connections.SendToAll(MessageCodes.UpdateState, new ServerStateMessage {
      Timestamp = timestamp,
      State = state
    }, NetworkReliablity.Unreliable);
  }

  void OnReceivedClientInput(NetworkDataMessage message) {
    if (ReceivedInputs == null) return;
    var inputSet = message.ReadAs<InputSetMessage>();
    ReceivedInputs(message.Connection.Id,
                   inputSet.StartTimestamp,
                   inputSet.Inputs);
  }

  public void Dispose() {
    // NetworkServer.UnregisterHandler(MessageCode.UpdateInput);
  }

}

}
