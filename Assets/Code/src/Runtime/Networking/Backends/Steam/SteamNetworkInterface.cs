using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Networking.Steam {

public sealed class SteamNetworkInterface : NetworkInterface {

  readonly IDictionary<int, CSteamID> connectionUsers;
  readonly IDictionary<CSteamID, int> connectionIds;

  Callback<P2PSessionRequest_t> callbackP2PSesssionRequest;
  Callback<P2PSessionConnectFail_t> callbackP2PConnectFail;
  Callback<LobbyChatUpdate_t> callbackLobbyChatUpdate;
  Callback<LobbyEnter_t> callbackLobbyEnter;
  
  const int kMaxMessageSize = 1200;
  int lastConnectionId;
  CSteamID? currentLobbyId;
  CSteamID? lobbyOwner;

  public SteamNetworkInterface() : base(kMaxMessageSize) {
    ValidateSteamInitalized();
    lastConnectionId = 0;
    connectionUsers = new Dictionary<int, CSteamID>();
    connectionIds = new Dictionary<CSteamID, int>();
  }

  public override async Task Initialize(NetworkInterfaceConfiguration config) {
    ValidateSteamInitalized();
    await base.Initialize(config);
    callbackP2PSesssionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
    callbackP2PConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
    callbackLobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    callbackLobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);

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

  public override async Task<NetworkConnection> Connect(NetworkConnectionConfig config) {
    ValidateSteamInitalized();
    if (config.LobbyID == null) {
      throw new ArgumentException("Cannot connect to Steam except through a lobby.");
    }
    var lobbyEnter = await SteamMatchmaking.JoinLobby(config.LobbyID.Value).ToTask<LobbyEnter_t>();
    var responseCode = (EChatRoomEnterResponse)lobbyEnter.m_EChatRoomEnterResponse;
    if (responseCode == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess) {
      currentLobbyId = new CSteamID(lobbyEnter.m_ulSteamIDLobby);
      lobbyOwner = SteamMatchmaking.GetLobbyOwner(currentLobbyId.Value);
      // Send initial message to process NAT traversal
      SteamNetworking.SendP2PPacket(lobbyOwner.Value, null, 0, EP2PSend.k_EP2PSendReliable);
      var connection = AddConnection(lobbyOwner.Value);
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
      CloseAllConnections();
    }
  }

  public override void Dispose() {
    CloseAllConnections();
    base.Dispose();
  }
  
  // Steam Callbacks

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

  void OnP2PSessionConnectFail(P2PSessionConnectFail_t evt) => OnDisconnect(evt.m_steamIDRemote);

  void OnLobbyEnter(LobbyEnter_t evt) {
    var lobbyId = new CSteamID(evt.m_ulSteamIDLobby);
    if (SteamMatchmaking.GetLobbyOwner(lobbyId) == SteamUser.GetSteamID()) {
      var success = SteamMatchmaking.SetLobbyData(lobbyId, "name", $"{SteamFriends.GetPersonaName()}'s Lobby");
      success |= SteamMatchmaking.SetLobbyData(lobbyId, "owner_name", SteamFriends.GetPersonaName());
      if (!success) {
        Debug.LogWarning("Error setting lobby info.");
      }
    }
  }

  void OnLobbyChatUpdate(LobbyChatUpdate_t evt) {
    if (currentLobbyId?.m_SteamID != evt.m_ulSteamIDLobby) return;
    var stateChange = (EChatMemberStateChange)evt.m_rgfChatMemberStateChange;
    if (stateChange == 0) {
      return;
    } else if ((stateChange | EChatMemberStateChange.k_EChatMemberStateChangeEntered) != 0) {
      OnChatJoin(evt);
    } else {
      OnDisconnect(new CSteamID(evt.m_ulSteamIDUserChanged));
    }
  }

  // Private Utility Functions

  void OnChatJoin(LobbyChatUpdate_t evt) {
    var user = new CSteamID(evt.m_ulSteamIDUserChanged);
    if (user == SteamUser.GetSteamID()) return; // Ignore join events involving self.
    Assert.IsTrue(!connectionIds.ContainsKey(user));
    AddConnection(user);
  }

  void OnDisconnect(CSteamID user) {
    if (user == SteamUser.GetSteamID()) {
      // User removed from lobby. 
    } else {
      if (user == lobbyOwner) {
        // Lobby owner has left, we need to leave the lobby to close it.
        CloseAllConnections();
      } else {
        RemoveConnection(user);
      }
    }
  }

  bool ExpectingClient(CSteamID id) {
    if (Config == null || currentLobbyId == null) return false;
    if (Config.Type == NetworkInterfaceType.Client) {
      // Only allow the lobby owner (the host/server) to connect to you if you are a client
      return lobbyOwner == id;
    } else {
      // Allow any user in the lobby to connect to the server
      var playerCount = SteamMatchmaking.GetNumLobbyMembers(currentLobbyId.Value);
      for (var i = 0; i < playerCount; i++) {
        if (SteamMatchmaking.GetLobbyMemberByIndex(currentLobbyId.Value, i) == id) return true;
      }
      return false;
    }
  }

  NetworkConnection AddConnection(CSteamID user) {
    connectionUsers[lastConnectionId] = user;
    connectionIds[user] = lastConnectionId;
    return OnNewConnection(lastConnectionId++);
  }

  void RemoveConnection(CSteamID user) {
    int id;
    if (!connectionIds.TryGetValue(user, out id)) return;
    SteamNetworking.CloseP2PSessionWithUser(user);
    connectionIds.Remove(user);
    connectionUsers.Remove(id);
  }

  void CloseAllConnections() {
    var lobby = currentLobbyId.Value;
    SteamMatchmaking.LeaveLobby(lobby);
    var playerCount = SteamMatchmaking.GetNumLobbyMembers(lobby);
    for (var i = 0; i < playerCount; i++) {
      var id = SteamMatchmaking.GetLobbyMemberByIndex(lobby, i);
      SteamNetworking.CloseP2PSessionWithUser(id);
    }
  }

  static void ValidateSteamInitalized() {
    if (!SteamManager.Initialized) {
      throw new InvalidOperationException("Cannot use Steam networking if the SteamManager is not initialzed.");
    }
  }

}

}