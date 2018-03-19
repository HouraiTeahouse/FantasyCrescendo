using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkGameServer : INetworkServer {

  readonly List<INetworkInterface> interfaces;

  public ICollection<NetworkClientPlayer> Clients => clients.Values;
  public event Action<int, uint, ArraySegment<MatchInput>> ReceivedInputs;
  public event Action<NetworkClientPlayer> PlayerAdded;
  public event Action<NetworkClientPlayer> PlayerUpdated;
  public event Action<int> PlayerRemoved;

  Dictionary<NetworkConnection, NetworkClientPlayer> clients;

  public int ClientCount => NetworkServer.connections.Count; 

  public NetworkGameServer(Type interfaceType, NetworkServerConfig config) {
    clients = new Dictionary<NetworkConnection, NetworkClientPlayer>();
    interfaces = new List<INetworkInterface>();
    var networkInterface = (INetworkInterface)Activator.CreateInstance(interfaceType);
    networkInterface.Initialize(new NetworkInterfaceConfiguration {
      Type = NetworkInterfaceType.Server,
      Port = config.Port
    });
    AddNetworkInterface(networkInterface);
  }

  void AddNetworkInterface(INetworkInterface networkInterface) {
    interfaces.Add(networkInterface);

    networkInterface.OnPeerConnected += OnConnect;
    networkInterface.OnPeerDisconnected += OnDisconnect;

    var handlers = networkInterface.MessageHandlers;
    handlers.RegisterHandler(MessageCodes.ClientReady, OnClientReady);
    handlers.RegisterHandler(MessageCodes.UpdateConfig, OnClientConfigUpdated);
    handlers.RegisterHandler(MessageCodes.UpdateInput, OnReceivedClientInput);
  }

  public void Update() {
    foreach (var networkInterface in interfaces) {
      networkInterface.Update();
    }
  }

  public void FinishMatch(MatchResult result) {
    foreach (var client in clients.Values) {
      client.FinishMatch(result);
    }
  }

	public void SetReady(bool ready) {
    foreach (var client in clients.Values) {
      client.SetServerReady(ready);
    }
	}

  public void BroadcastInput(uint timestamp, byte validMask, IEnumerable<MatchInput> inputs) {
    foreach (var client in clients.Values) {
      client.SendInputs(timestamp, validMask, inputs);
    }
  }

  public void BroadcastState(uint timestamp, MatchState state, MatchInput? latestInput = null) {
    foreach (var client in clients.Values) {
      client.SendState(timestamp, state, latestInput);
    }
  }

  public void Dispose() {
    if (interfaces.Count <= 0) return;
    foreach (var networkInterface in interfaces) {
      networkInterface.Dispose();
      networkInterface.OnPeerConnected -= OnConnect;
      networkInterface.OnPeerConnected -= OnConnect;

      var handlers = networkInterface.MessageHandlers;
      if (handlers == null) return;
      handlers.RegisterHandler(MessageCodes.ClientReady, OnClientReady);
      handlers.RegisterHandler(MessageCodes.UpdateInput, OnReceivedClientInput);
    }
    interfaces.Clear();
  }

  public INetworkClient CreateLocalClient() {
    // TODO(james7132): Make this dynamic
    var clientToServer = new LocalInterface();
    var serverInterface = clientToServer.Mirror;
    var serverToClient = serverInterface.Connection;
    var localClient = new NetworkClientPlayer(serverToClient, LowestAvailablePlayerID(serverToClient));
    localClient.Config.PlayerID = localClient.PlayerID;
    clients[serverToClient] = localClient;
    PlayerAdded?.Invoke(localClient);
    AddNetworkInterface(serverInterface);
    return new NetworkGameClient(clientToServer.Connection);
  }

  // Event Handlers

  void OnConnect(NetworkConnection connection) {
    var client = new NetworkClientPlayer(connection, LowestAvailablePlayerID(connection));
    client.Config.PlayerID = client.PlayerID;
    clients[connection] = client;
    PlayerAdded?.Invoke(client);
  }

  void OnDisconnect(NetworkConnection connection) {
    NetworkClientPlayer client;
    if (!clients.TryGetValue(connection, out client)) return;
    clients.Remove(connection);
    InvokePlayerRemoved(client.PlayerID);
  }

  void OnClientReady(NetworkDataMessage dataMsg) {
    NetworkClientPlayer client;
    if (!clients.TryGetValue(dataMsg.Connection, out client)) return;
    var message = dataMsg.ReadAs<PeerReadyMessage>();
    client.IsReady = message.IsReady;
    InvokePlayerUpdated(client);
  }

  void OnClientConfigUpdated(NetworkDataMessage dataMsg) {
    NetworkClientPlayer client;
    if (!clients.TryGetValue(dataMsg.Connection, out client)) return;
    var message = dataMsg.ReadAs<ClientUpdateConfigMessage>();
    client.Config = message.PlayerConfig;
    client.Config.PlayerID = client.PlayerID;
    InvokePlayerUpdated(client);
  }

  void OnReceivedClientInput(NetworkDataMessage message) {
    NetworkClientPlayer client;
    if (!clients.TryGetValue(message.Connection, out client)) return;
    var inputSet = message.ReadAs<InputSetMessage>();
    InvokeRecievedInputs(client.PlayerID, inputSet.StartTimestamp,
                         inputSet.AsArraySegment());
  }

  internal void InvokePlayerRemoved(int id) {
    PlayerRemoved?.Invoke(id);
  }

  internal void InvokePlayerUpdated(NetworkClientPlayer client) {
    PlayerUpdated?.Invoke(client);
  }

  internal void InvokeRecievedInputs(int connectionId, uint timestamp, ArraySegment<MatchInput> input) {
    ReceivedInputs?.Invoke(connectionId, timestamp, input);
  }

  byte LowestAvailablePlayerID(NetworkConnection connection) {
    bool updated = false;
    byte id = 0;
    do {
      updated = false;
      foreach (var kvp in clients) {
        if (kvp.Key == connection) continue;
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
