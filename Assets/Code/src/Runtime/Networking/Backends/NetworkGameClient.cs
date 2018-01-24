using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkGameClient : INetworkClient {

  public event Action<MatchConfig> OnMatchStarted;
  public event Action<MatchResult> OnMatchFinished;

  public event Action<MatchConfig> OnMatchConfigUpdated;

  public event Action<uint, IEnumerable<MatchInput>> OnRecievedInputs;
  public event Action<uint, MatchState> OnRecievedState;

  readonly INetworkInterface NetworkInterface;
  INetworkConnection ServerConnection;
  TaskCompletionSource<object> connectionTask;

  public NetworkGameClient(Type interfaceType, NetworkClientConfig config) {
    NetworkInterface = (INetworkInterface)Activator.CreateInstance(interfaceType);
    NetworkInterface.Initialize(0);
  }

  public void Update() => NetworkInterface.Update();

  public async Task Connect(string ip, uint port) {
    if (connectionTask != null) {
      await connectionTask.Task;
      return;
    }
    connectionTask = new TaskCompletionSource<object>();
    ServerConnection = await NetworkInterface.Connect(ip, (int)port);

    var handlers = ServerConnection.MessageHandlers;
    handlers.RegisterHandler<InputSetMessage>(MessageCodes.UpdateInput, OnGetInput);
    handlers.RegisterHandler<ServerStateMessage>(MessageCodes.UpdateState, OnGetState);
    handlers.RegisterHandler<MatchStartMessage>(MessageCodes.MatchStart, OnStartMatch);
    handlers.RegisterHandler<MatchFinishMessage>(MessageCodes.MatchFinish, OnFinishMatch);
    handlers.RegisterHandler<ServerUpdateConfigMessage>(MessageCodes.UpdateConfig, OnUpdateConfig);

    connectionTask.TrySetResult(new object());
  }

  public void Disconnect() => ServerConnection?.Disconnect();

  public void SetReady(bool isReady) {
    ServerConnection?.Send(MessageCodes.ClientReady, new ClientReadyMessage {
      IsReady = isReady
    });
  }

  public void SetConfig(PlayerConfig config) {
    ServerConnection?.Send(MessageCodes.UpdateConfig, new ClientUpdateConfigMessage {
      PlayerConfig = config
    });
  }

  public void SendInput(uint startTimestamp, IEnumerable<MatchInput> input) {
    ServerConnection?.Send(MessageCodes.UpdateInput, new InputSetMessage {
      StartTimestamp = startTimestamp,
      Inputs = input.ToArray()
    }, NetworkReliablity.Unreliable);
  }

  public void Dispose() {
    NetworkInterface.Dispose();
    var handlers = ServerConnection?.MessageHandlers;
    if (handlers == null) return;
    handlers.UnregisterHandler(MessageCodes.UpdateInput);
    handlers.UnregisterHandler(MessageCodes.UpdateState);
    handlers.UnregisterHandler(MessageCodes.MatchStart);
    handlers.UnregisterHandler(MessageCodes.MatchFinish);
    handlers.UnregisterHandler(MessageCodes.UpdateConfig);
  }

  // Event Handlers

  void OnStartMatch(MatchStartMessage message) => OnMatchStarted?.Invoke(message.MatchConfig);
  void OnFinishMatch(MatchFinishMessage message) => OnMatchFinished?.Invoke(message.MatchResult);
  void OnUpdateConfig(ServerUpdateConfigMessage message) => OnMatchConfigUpdated?.Invoke(message.MatchConfig);
  void OnGetState(ServerStateMessage message) => OnRecievedState?.Invoke(message.Timestamp, message.State);
  void OnGetInput(InputSetMessage message) => OnRecievedInputs?.Invoke(message.StartTimestamp, message.Inputs);

}

}
