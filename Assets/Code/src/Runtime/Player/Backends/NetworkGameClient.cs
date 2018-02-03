using HouraiTeahouse.FantasyCrescendo.Matches;
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

  public event Action<uint, ArraySlice<MatchInput>> OnRecievedInputs;
  public event Action<uint, MatchState> OnRecievedState;
  public event Action<bool> OnServerReady;

  public bool IsServerReady { get; private set; }

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
    handlers.RegisterHandler<PeerReadyMessage>(MessageCodes.ServerReady, OnSetServerReady);

    connectionTask.TrySetResult(new object());
  }

  public void Disconnect() => ServerConnection?.Disconnect();

  public void SetReady(bool isReady) {
    if (ServerConnection == null) return;
    using (var rental = ObjectPool<PeerReadyMessage>.Shared.Borrow()) {
      rental.RentedObject.IsReady = isReady;
      ServerConnection.Send(MessageCodes.ClientReady, rental.RentedObject);
    }
  }

  public void SetConfig(PlayerConfig config) {
    if (ServerConnection == null) return;
    using (var rental = ObjectPool<ClientUpdateConfigMessage>.Shared.Borrow()) {
      rental.RentedObject.PlayerConfig = config;
      ServerConnection.Send(MessageCodes.UpdateConfig, rental.RentedObject);
    }
  }

  public void SendInput(uint startTimestamp, byte validMask, IEnumerable<MatchInput> input) {
    if (ServerConnection == null) return;
    int inputCount;
    var inputs = ArrayUtil.ConvertToArray(input, out inputCount);
    if (inputCount <= 0) return;
    using (var rental = ObjectPool<InputSetMessage>.Shared.Borrow()) {
      var message = rental.RentedObject;
      message.StartTimestamp = startTimestamp;
      message.InputCount = (uint)inputCount;
      message.ValidMask = validMask;
      message.Inputs = inputs;
      ServerConnection.Send(MessageCodes.UpdateInput, message, NetworkReliablity.Unreliable);
    }
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
  void OnGetInput(InputSetMessage message) {
    OnRecievedInputs?.Invoke(message.StartTimestamp, message.Inputs.GetSlice(message.InputCount)); 
  }
  void OnSetServerReady(PeerReadyMessage message) {
    IsServerReady = message.IsReady;
    OnServerReady?.Invoke(IsServerReady);
  }

}

}
