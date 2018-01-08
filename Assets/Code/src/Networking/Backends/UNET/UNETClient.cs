using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public class UNETClient : INetworkClient {

  public event Action<uint, IEnumerable<GameInput>> ReceivedInputs;
  public event Action<uint, GameState> ReceivedState;

  readonly NetworkClient unetClient;

  public UNETClient(NetworkClient client) {
    unetClient = client;

    unetClient.RegisterHandler(MessageCode.UpdateState, OnReceivedState);
    unetClient.RegisterHandler(MessageCode.UpdateInput, OnReceivedInput);
  }

  public void SendInput(uint startTimestamp, IEnumerable<GameInput> input) {
    unetClient.SendUnreliable(MessageCode.UpdateInput, new InputSetMessage {
        StartTimestamp = startTimestamp,
        Inputs = input.ToArray()
    });
  }

  void OnReceivedState(NetworkMessage message) {
    if (ReceivedState == null) {
      return;
    }
    var serverState = message.ReadMessage<ServerStateMessage>();
    ReceivedState(serverState.Timestamp, serverState.State);
  }

  void OnReceivedInput(NetworkMessage message) {
    if (ReceivedInputs == null) {
      return;
    }
    var inputs = message.ReadMessage<InputSetMessage>();
    ReceivedInputs(inputs.StartTimestamp, inputs.Inputs);
  }

  public void Dispose() {
    unetClient.UnregisterHandler(MessageCode.UpdateState);
    unetClient.UnregisterHandler(MessageCode.UpdateInput);
  }

}

}
