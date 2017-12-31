using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public class UNETClient : INetworkClient {

  public event Action<uint, IEnumerable<GameInput>> RecievedInputs;
  public event Action<uint, GameState> RecievedState;

  readonly NetworkClient unetClient;

  public UNETClient(NetworkClient client) {
    unetClient = client;

    unetClient.RegisterHandler(MessageCode.UpdateState, OnRecievedState);
    unetClient.RegisterHandler(MessageCode.UpdateInput, OnRecievedInput);
  }

  public void SendInput(uint startTimestamp, IEnumerable<GameInput> input) {
    unetClient.SendUnreliable(MessageCode.UpdateInput, new InputSetMessage {
        StartTimestamp = startTimestamp,
        Inputs = input.ToArray()
    });
  }

  void OnRecievedState(NetworkMessage message) {
    if (RecievedState == null) {
      return;
    }
    var serverState = message.ReadMessage<ServerStateMessage>();
    RecievedState(serverState.Timestamp, serverState.State);
  }

  void OnRecievedInput(NetworkMessage message) {
    if (RecievedInputs == null) {
      return;
    }
    var inputs = message.ReadMessage<InputSetMessage>();
    RecievedInputs(inputs.StartTimestamp, inputs.Inputs);
  }

  public void Dispose() {
    unetClient.UnregisterHandler(MessageCode.UpdateState);
    unetClient.UnregisterHandler(MessageCode.UpdateInput);
  }

}

}
