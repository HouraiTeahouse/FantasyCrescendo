using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkGameServer : INetworkServer {

  readonly INetworkInterface NetworkInterface;

  public ICollection<NetworkClientPlayer> Clients => clients.Values;
  public event Action<int, uint, ArraySegment<MatchInput>> ReceivedInputs;
  public event Action<NetworkClientPlayer> PlayerAdded;
  public event Action<NetworkClientPlayer> PlayerUpdated;
  public event Action<int> PlayerRemoved;

  Dictionary<int, NetworkClientPlayer> clients;

  public int ClientCount => NetworkServer.connections.Count; 

  public NetworkGameServer(Type interfaceType, NetworkServerConfig config) {
    clients = new Dictionary<int, NetworkClientPlayer>();
    NetworkInterface = (INetworkInterface)Activator.CreateInstance(interfaceType);
    NetworkInterface.Initialize(new NetworkInterfaceConfiguration {
      Type = NetworkInterfaceType.Server,
      Port = config.Port
    });

    NetworkInterface.OnPeerConnected += OnConnect;
    NetworkInterface.OnPeerDisconnected += OnDisconnect;

    var handlers = NetworkInterface.MessageHandlers;
    handlers.RegisterHandler(MessageCodes.ClientReady, OnClientReady);
    handlers.RegisterHandler(MessageCodes.UpdateConfig, OnClientConfigUpdated);
    handlers.RegisterHandler(MessageCodes.UpdateInput, OnReceivedClientInput);
  }

  public void Update() => NetworkInterface.Update();

  public void FinishMatch(MatchResult result) {
    NetworkInterface.Connections.SendToAll(MessageCodes.MatchFinish, new MatchFinishMessage {
      MatchResult = result
    });
  }

	public void SetReady(bool ready) {
    NetworkInterface.Connections.SendToAll(MessageCodes.ServerReady, new PeerReadyMessage {
      IsReady = ready
    });
	}

  public void BroadcastInput(uint startTimestamp, byte validMask, IEnumerable<MatchInput> input) {
    int inputCount;
    var inputs = ArrayUtil.ConvertToArray(input, out inputCount);
    if (inputCount <= 0) return;
    NetworkInterface.Connections.SendToAll(MessageCodes.UpdateInput, new InputSetMessage {
      StartTimestamp = startTimestamp,
      InputCount = (uint)inputCount,
      ValidMask = validMask,
      Inputs = inputs
    }, NetworkReliablity.Unreliable);
  }

  public void BroadcastState(uint timestamp, MatchState state, MatchInput? latestInput = null) {
    NetworkInterface.Connections.SendToAll(MessageCodes.UpdateState, new ServerStateMessage {
      Timestamp = timestamp,
      State = state,
      LatestInput = latestInput
    }, NetworkReliablity.Unreliable);
  }

  public void Dispose() {
    if (NetworkInterface == null) return;
    NetworkInterface.Dispose();
    NetworkInterface.OnPeerConnected -= OnConnect;
    NetworkInterface.OnPeerConnected -= OnConnect;

    var handlers = NetworkInterface.MessageHandlers;
    if (handlers == null) return;
    handlers.RegisterHandler(MessageCodes.ClientReady, OnClientReady);
    handlers.RegisterHandler(MessageCodes.UpdateInput, OnReceivedClientInput);
  }

  // Event Handlers

  void OnConnect(NetworkConnection connection) {
    var connId = connection.Id;
    var client = new NetworkClientPlayer(connection, LowestAvailablePlayerID(connId));
    client.Config.PlayerID = client.PlayerID;
    clients[connId] = client;
    PlayerAdded?.Invoke(client);
  }

  void OnDisconnect(NetworkConnection connection) {
    clients.Remove(connection.Id);
    PlayerRemoved?.Invoke(connection.Id);
  }

  void OnClientReady(NetworkDataMessage dataMsg) {
    NetworkClientPlayer client;
    if (!clients.TryGetValue(dataMsg.Connection.Id, out client)) return;
    var message = dataMsg.ReadAs<PeerReadyMessage>();
    client.IsReady = message.IsReady;
    PlayerUpdated?.Invoke(client);
  }

  void OnClientConfigUpdated(NetworkDataMessage dataMsg) {
    NetworkClientPlayer client;
    if (!clients.TryGetValue(dataMsg.Connection.Id, out client)) return;
    var message = dataMsg.ReadAs<ClientUpdateConfigMessage>();
    client.Config = message.PlayerConfig;
    client.Config.PlayerID = client.PlayerID;
    PlayerUpdated?.Invoke(client);
  }

  void OnReceivedClientInput(NetworkDataMessage message) {
    if (ReceivedInputs == null) return;
    var inputSet = message.ReadAs<InputSetMessage>();
    ReceivedInputs(message.Connection.Id, inputSet.StartTimestamp,
                   inputSet.AsArraySegment());
  }

  byte LowestAvailablePlayerID(int connectionId) {
    bool updated = false;
    byte id = 0;
    do {
      updated = false;
      foreach (var kvp in clients) {
        if (kvp.Key == connectionId) continue;
        var client = kvp.Value;
        if (client.PlayerID == id) {
          id++;
          updated = true;
        }
      }
    } while (updated);
    return id;
  }

}

}
