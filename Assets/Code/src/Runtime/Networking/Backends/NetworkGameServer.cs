using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkGameServer : INetworkServer {

  readonly INetworkInterface NetworkInterface;

  public ICollection<NetworkClientPlayer> Clients => clients.Values;
  public event Action<uint, uint, IEnumerable<MatchInput>> ReceivedInputs;
  public event Action<NetworkClientPlayer> PlayerAdded;
  public event Action<NetworkClientPlayer> PlayerUpdated;
  public event Action<uint> PlayerRemoved;

  Dictionary<uint, NetworkClientPlayer> clients;

  public int ClientCount => NetworkServer.connections.Count; 

  public NetworkGameServer(Type interfaceType, NetworkServerConfig config) {
    clients = new Dictionary<uint, NetworkClientPlayer>();
    NetworkInterface = (INetworkInterface)Activator.CreateInstance(interfaceType);
    NetworkInterface.Initialize(config.Port);

    NetworkInterface.OnPeerConnected += OnConnect;
    NetworkInterface.OnPeerDisconnected += OnDisconnect;

    var handlers = NetworkInterface.MessageHandlers;
    handlers.RegisterHandler(MessageCodes.PeerReady, OnClientReady);
    handlers.RegisterHandler(MessageCodes.UpdateConfig, OnClientConfigUpdated);
    handlers.RegisterHandler(MessageCodes.UpdateInput, OnReceivedClientInput);
  }

  public void Update() => NetworkInterface.Update();

  public void StartMatch(MatchConfig config) {
    NetworkInterface.Connections.SendToAll(MessageCodes.MatchStart, new MatchStartMessage {
      MatchConfig = config
    });
  }

  public void FinishMatch(MatchResult result) {
    NetworkInterface.Connections.SendToAll(MessageCodes.MatchFinish, new MatchFinishMessage {
      MatchResult = result
    });
  }

	public void SetReady(bool ready) {
    NetworkInterface.Connections.SendToAll(MessageCodes.PeerReady, new PeerReadyMessage {
			IsReady = ready
    });
	}

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

  public MatchConfig BuildMatchConfigForClient(MatchConfig baseConfig, NetworkClientPlayer clientPlayer) {
    // TODO(james7132): Generalize this to work with multiple players per client
    baseConfig.PlayerConfigs = (from client in Clients orderby client.PlayerID select client.Config).ToArray();
    for (var i = 0; i < baseConfig.PlayerConfigs.Length; i++) {
      baseConfig.PlayerConfigs[i].LocalPlayerID = -1;
    }
    baseConfig.PlayerConfigs[clientPlayer.PlayerID].LocalPlayerID = 1;
    return baseConfig;
  }

  public void Dispose() {
    if (NetworkInterface == null) return;
    NetworkInterface.Dispose();
    NetworkInterface.OnPeerConnected -= OnConnect;
    NetworkInterface.OnPeerConnected -= OnConnect;

    var handlers = NetworkInterface.MessageHandlers;
    if (handlers == null) return;
    handlers.RegisterHandler(MessageCodes.PeerReady, OnClientReady);
    handlers.RegisterHandler(MessageCodes.UpdateInput, OnReceivedClientInput);
  }

  // Event Handlers

  void OnConnect(INetworkConnection connection) {
    var connId = connection.Id;
    var client = new NetworkClientPlayer(connection, LowestAvailablePlayerID(connId));
    clients[connId] = client;
    PlayerAdded?.Invoke(client);
  }

  void OnDisconnect(INetworkConnection connection) {
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
    PlayerUpdated?.Invoke(client);
  }

  void OnReceivedClientInput(NetworkDataMessage message) {
    if (ReceivedInputs == null) return;
    var inputSet = message.ReadAs<InputSetMessage>();
    ReceivedInputs(message.Connection.Id,
                   inputSet.StartTimestamp,
                   inputSet.Inputs);
  }

  uint LowestAvailablePlayerID(uint connectionId) {
    bool updated = false;
    uint id = 0;
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
