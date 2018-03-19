using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking.Steam {

public sealed class SteamNetworkInterface : NetworkInterface {

  readonly IDictionary<int, CSteamID> connectionUsers;
  readonly IDictionary<CSteamID, int> connectionIds;

  Callback<P2PSessionRequest_t> callbackP2PSesssionRequest;
  Callback<P2PSessionConnectFail_t> callbackP2PConnectFail;
  Callback<LobbyChatUpdate_t> callbackLobbyChatUpdate;
  
  const int kMaxMessageSize = 1200;
  int lastConnectionId;
  CSteamID? currentLobbyId;

  public SteamNetworkInterface() : base(kMaxMessageSize) {
    ValidateSteamInitalized();
    lastConnectionId = 0;
    connectionUsers = new Dictionary<int, CSteamID>();
    connectionIds = new Dictionary<CSteamID, int>();
  }

  public override async Task Initialize(NetworkInterfaceConfiguration config) {
    await base.Initialize(config);
    ValidateSteamInitalized();
    callbackP2PSesssionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
    callbackP2PConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
    callbackLobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);

    if (config.Type == NetworkInterfaceType.Server) {
      var type = config.ServerSteamLobbyType;
      var size = config.ServerSteamLobbyMaxSize;
      var result = await SteamMatchmaking.CreateLobby(type, size).ToTask<LobbyCreated_t>();
      
      if (!SteamUtility.IsError(result.m_eResult)) {
        currentLobbyId = new CSteamID(result.m_ulSteamIDLobby);
      } else {
        throw SteamUtility.CreateError(result.m_eResult);
      }
    }
  }

  void OnP2PSessionRequest(P2PSessionRequest_t evt) {
    var id = evt.m_steamIDRemote;
    if (ExpectingClient(id)) {
      AddConnection(id);
      // Send response packet to ack the connection creation
      SteamNetworking.SendP2PPacket(id, null, 0, EP2PSend.k_EP2PSendReliable);
    } else {
      Debug.LogWarning("Unexpected session request from " + id);
    }
  }

  void OnP2PSessionConnectFail(P2PSessionConnectFail_t evt) {
    var user = evt.m_steamIDRemote;

    int connectionId;
    if (!connectionIds.TryGetValue(user, out connectionId)) return;
    connectionIds.Remove(user);
    connectionUsers.Remove(connectionId);

    OnDisconnect(connectionId, null);
  }

  void OnLobbyChatUpdate(LobbyChatUpdate_t evt) {
  }

  bool ExpectingClient(CSteamID id) {
    if (currentLobbyId == null) return false;
    // Check if player is in the lobby 
    var playerCount = SteamMatchmaking.GetNumLobbyMembers(currentLobbyId.Value);
    for (var i = 0; i < playerCount; i++) {
      if (SteamMatchmaking.GetLobbyMemberByIndex(currentLobbyId.Value, i) == id) return true;
    }
    return false;
  }

  public override async Task<NetworkConnection> Connect(NetworkConnectionConfig config) {
    var lobbyEnter = await SteamMatchmaking.JoinLobby(config.LobbyID.Value).ToTask<LobbyEnter_t>();
    var responseCode = (EChatRoomEnterResponse)lobbyEnter.m_EChatRoomEnterResponse;
    if (responseCode == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess) {
      currentLobbyId = new CSteamID(lobbyEnter.m_ulSteamIDLobby);
      var owner = SteamMatchmaking.GetLobbyOwner(currentLobbyId.Value);
      // Send initial message to process NAT traversal
      SteamNetworking.SendP2PPacket(owner, null, 0, EP2PSend.k_EP2PSendReliable);
      var connection = AddConnection(owner);
      await connection.ConnectTask.Task;
      return connection;
    } else {
      throw new Exception($"Could not join lobby {lobbyEnter.m_ulSteamIDLobby}. Error: {responseCode}.");
    }
  }

  protected override void SendImpl(int connectionId, byte[] buffer, int count, 
                                   NetworkReliablity reliability) {
    ValidateSteamInitalized();
    EP2PSend ep2p = reliability == NetworkReliablity.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable;
    SteamNetworking.SendP2PPacket(connectionUsers[connectionId], buffer, (uint)count, ep2p);
  }

  public override void Update() {
    ValidateSteamInitalized();
    uint dataSize;
    CSteamID userId;
    while (SteamNetworking.ReadP2PPacket(ReadBuffer, (uint)ReadBuffer.Length, out dataSize, out userId)) {
      OnRecieveData(connectionIds[userId], ReadBuffer, (int)dataSize);
    }
  }

  public override void Disconnect(int connectionId) {
    ValidateSteamInitalized();
    if (currentLobbyId != null) {
      var lobby = currentLobbyId.Value;
      SteamMatchmaking.LeaveLobby(lobby);
      var playerCount = SteamMatchmaking.GetNumLobbyMembers(lobby);
      for (var i = 0; i < playerCount; i++) {
        var id = SteamMatchmaking.GetLobbyMemberByIndex(lobby, i);
        SteamNetworking.CloseP2PSessionWithUser(id);
      }
    }
  }

  NetworkConnection AddConnection(CSteamID user) {
    connectionUsers[lastConnectionId] = user;
    connectionIds[user] = lastConnectionId;
    return OnNewConnection(lastConnectionId++);
  }

  static void ValidateSteamInitalized() {
    if (!SteamManager.Initialized) {
      throw new InvalidOperationException("Cannot use Steam networking if the SteamManager is not initialzed.");
    }
  }

}

}