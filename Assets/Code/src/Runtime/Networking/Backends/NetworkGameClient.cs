using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkGameClient : INetworkClient {

  public NetworkConnection Connection { get; private set; }

  public event Action<MatchConfig> OnMatchStarted;
  public event Action<MatchResult> OnMatchFinished;

  public event Action<MatchConfig> OnMatchConfigUpdated;

  public event Action<uint, ArraySegment<MatchInput>> OnRecievedInputs;
  public event Action<uint, MatchState, MatchInput?> OnRecievedState;
  public event Action<bool> OnServerReady;

  public bool IsServerReady { get; private set; }

  readonly INetworkInterface NetworkInterface;
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
    Connection = await NetworkInterface.Connect(ip, (int)port);

    var handlers = Connection.MessageHandlers;
    handlers.RegisterHandler<InputSetMessage>(MessageCodes.UpdateInput, OnGetInput);
    handlers.RegisterHandler<ServerStateMessage>(MessageCodes.UpdateState, OnGetState);
    handlers.RegisterHandler<MatchStartMessage>(MessageCodes.MatchStart, OnStartMatch);
    handlers.RegisterHandler<MatchFinishMessage>(MessageCodes.MatchFinish, OnFinishMatch);
    handlers.RegisterHandler<ServerUpdateConfigMessage>(MessageCodes.UpdateConfig, OnUpdateConfig);
    handlers.RegisterHandler<PeerReadyMessage>(MessageCodes.ServerReady, OnSetServerReady);

    connectionTask.TrySetResult(new object());
  }

  public void Disconnect() => Connection?.Disconnect();

  public void SetReady(bool isReady) {
    if (Connection == null) return;
    Connection.Send(MessageCodes.ClientReady, new PeerReadyMessage {
      IsReady = isReady
    });
  }

  public void SetConfig(PlayerConfig config) {
    if (Connection == null) return;
    Connection.Send(MessageCodes.UpdateConfig, new ClientUpdateConfigMessage {
      PlayerConfig = config
    });
  }

  public void SendInput(uint startTimestamp, byte validMask, IEnumerable<MatchInput> input) {
    if (Connection == null) return;
    int inputCount;
    var inputs = ArrayUtil.ConvertToArray(input, out inputCount);
    if (inputCount <= 0) return;
    Connection.Send(MessageCodes.UpdateInput, new InputSetMessage {
      StartTimestamp = startTimestamp,
      InputCount = (uint)inputCount,
      ValidMask = validMask,
      Inputs = inputs
    }, NetworkReliablity.Unreliable);
  }

  public void Dispose() {
    NetworkInterface.Dispose();
    var handlers = Connection?.MessageHandlers;
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
  void OnGetState(ServerStateMessage message) => OnRecievedState?.Invoke(message.Timestamp, message.State, message.LatestInput);
  void OnGetInput(InputSetMessage message) {
    OnRecievedInputs?.Invoke(message.StartTimestamp, message.AsArraySegment()); 
  }
  void OnSetServerReady(PeerReadyMessage message) {
    IsServerReady = message.IsReady;
    OnServerReady?.Invoke(IsServerReady);
  }

}

}
