using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkGameClient : INetworkClient {

  public event Action<uint, IEnumerable<MatchInput>> ReceivedInputs;
  public event Action<uint, MatchState> ReceivedState;

  readonly INetworkInterface NetworkInterface;
  INetworkConnection ServerConnection;
  TaskCompletionSource<object> connectionTask;

  public NetworkGameClient(INetworkInterface networkInterface, NetworkClientConfig config) {
    NetworkInterface = networkInterface;
  }

  public async Task Connect(string ip, uint port) {
    if (connectionTask != null) {
      await connectionTask.Task;
      return;
    }
    connectionTask = new TaskCompletionSource<object>();
    ServerConnection = await NetworkInterface.Connect(ip, (int)port);
    
    var handlers = ServerConnection.MessageHandlers;
    handlers.RegisterHandler<InputSetMessage>(MessageCodes.UpdateInput, OnReceivedInput);
    handlers.RegisterHandler<ServerStateMessage>(MessageCodes.UpdateState, OnReceivedState);

    connectionTask.TrySetResult(new object());
    // unetClient.RegisterHandler(MsgType.Connect, OnConnect);
    // unetClient.RegisterHandler(MsgType.Disconnect, OnDisconnect);
  }

  public void Disconnect() {
  }

  public void SendInput(uint startTimestamp, IEnumerable<MatchInput> input) {
    ServerConnection.Send(MessageCodes.UpdateInput, new InputSetMessage {
      StartTimestamp = startTimestamp,
      Inputs = input.ToArray()
    }, NetworkReliablity.Unreliable);
  }

  public void Dispose() {
    var handlers = ServerConnection?.MessageHandlers;
    if (handlers == null) return;
    handlers.UnregisterHandler(MessageCodes.UpdateInput);
    handlers.UnregisterHandler(MessageCodes.UpdateState);
  }

  // Event Handlers

  void OnReceivedState(ServerStateMessage message) => ReceivedState?.Invoke(message.Timestamp, message.State);
  void OnReceivedInput(InputSetMessage message) => ReceivedInputs?.Invoke(message.StartTimestamp, message.Inputs);

}

}
